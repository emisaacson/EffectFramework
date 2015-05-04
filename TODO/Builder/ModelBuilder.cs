using HRMS.Core.Attributes;
using HRMS.Core.Helpers;
using HRMS.Core.Security;
using HRMS.Core.Extensions;
using HRMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.ComponentModel;
using HRMS.Modules.DBModel;
using HRMS.Modules.DBModel.Local;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic;
using System.Data.Linq;

namespace HRMS.Core.Builders
{
    public class ModelBuilder<ModelT> : ModelBuilderBase
        where ModelT : Model, new()
    {

        protected string ContextPropertyName;
        protected Type tDBT;

        public IBuilderSpecification BuilderSpecification { get; set; }

        public ModelBuilder(LocalDataClassesDataContext Local, HrmsMasterDataClassesDataContext Master, string CipherKey)
            : base(Local, Master, CipherKey)
        {
            ITableSpecifier TableInfo;
            var LocalTableInfo = typeof(ModelT).GetCustomAttribute<LocalTable>();
            var MasterTableInfo = typeof(ModelT).GetCustomAttribute<MasterTable>();

            if (MasterTableInfo != null)
            {
                TableInfo = (ITableSpecifier)MasterTableInfo;
            }
            else if (LocalTableInfo != null)
            {
                TableInfo = (ITableSpecifier)LocalTableInfo;
            }
            else
            {
                throw new ArgumentException("Can't build it if we don't know how.");
            }

            this.ContextPropertyName = TableInfo.TablePluralName;
            this.tDBT = TableInfo.TableType;
        }

        public ModelBuilder(LocalDataClassesDataContext Local, HrmsMasterDataClassesDataContext Master) : this(Local,Master,"")
        {
            
        }
        

        public virtual ModelCollection<ModelT> BuildModelFromEmpMasterID(int EmpMasterID, bool ClearCache = false)
        {
            if (DBHelper.IsSelfService() && DBHelper.GetCurrentEmpMasterId() != EmpMasterID)
            {
                throw new HttpException(403, "Unauthorized");
            }

            this.Record = new ModelCollection<ModelT>();

            var MasterLocalEmployee = new _MasterLocalEmployee(EmpMasterID);
            this.EmployeeMaster = MasterLocalEmployee.EmployeeMaster;
            this.EmployeeLocal = MasterLocalEmployee.EmployeeLocal;

            ((ModelCollection<ModelT>)this.Record).Items = (IEnumerable<ModelT>)this.GetType()
                                                                                    .GetMethod("FillItems", BindingFlags.NonPublic | BindingFlags.Instance)
                                                                                    .MakeGenericMethod(this.tDBT)
                                                                                    .Invoke(this, new object[] { ClearCache });

            return (ModelCollection<ModelT>)this.Record;
        }

        public ModelCollection<ModelT> SaveModelCollectionToDB(int EmpMasterID, ModelCollection<ModelT> Models)
        {
            if (DBHelper.IsSelfService() && EmpMasterID != DBHelper.GetCurrentEmpMasterId())
            {
                throw new HttpException(403, "Unauthorized Access");
            }

            return (ModelCollection<ModelT>)this.GetType()
                                                .GetMethods()
                                                .Where(x => x.Name == "SaveModelCollectionToDB" && x.IsGenericMethod)
                                                .First()
                                                .MakeGenericMethod(this.tDBT)
                                                .Invoke(this, new object[] { EmpMasterID, Models });
        }

        public virtual ModelCollection<ModelT> SaveModelCollectionToDB<DBT>(int EmpMasterID, ModelCollection<ModelT> Models)
            where DBT : class, new()
        {
            if (DBHelper.IsSelfService() && EmpMasterID != DBHelper.GetCurrentEmpMasterId())
            {
                throw new HttpException(403, "Unauthorized Access");
            }

            Logger Log = new Logger(EmpMasterID);

            var MasterLocalEmployee = new _MasterLocalEmployee(EmpMasterID);
            this.EmployeeMaster = MasterLocalEmployee.EmployeeMaster;
            this.EmployeeLocal = MasterLocalEmployee.EmployeeLocal;

            var PrimaryKey = this.GetModelPrimaryKey();
            var EmpGeneralIDPropertyName = this.GetEmpGeneralIDPropertyName();
            var DBPrimaryKey = typeof(ModelT).GetProperty(PrimaryKey.Name).GetCustomAttribute<LocalProperty>(false).ColumnName ?? PrimaryKey.Name;

            this.DeleteDeletedItems<DBT>(Models);

            List<DBT> InsertedItems = new List<DBT>();
            if (Models != null)
            {
                foreach (var Item in Models.Items)
                {
                    var ModelProperties = Item.GetType().GetProperties();
                    DBT DBItem = default(DBT);
                    if (((int?)(Item.GetType().GetProperty(PrimaryKey.Name).GetValue(Item))).HasValue)
                    {
                        var KeyValue = (int)(Item.GetType().GetProperty(PrimaryKey.Name).GetValue(Item));
                        if (EmpGeneralIDPropertyName != null)
                        {
                            DBItem = ((IEnumerable<DBT>)((Table<DBT>)(LocalDB.GetType().GetProperty(this.ContextPropertyName).GetValue(LocalDB)))
                                        .Where(DBPrimaryKey + "=@0 AND " + EmpGeneralIDPropertyName + "=@1", KeyValue, EmployeeHRMS.CoreEmpGeneralID)).First();
                        }
                        else
                        {
                            DBItem = ((IEnumerable<DBT>)((Table<DBT>)(LocalDB.GetType().GetProperty(this.ContextPropertyName).GetValue(LocalDB)))
                                        .Where(DBPrimaryKey + "=@0", KeyValue)).First();
                        }
                    }
                    else
                    {
                        DBItem = new DBT();
                        if (typeof(DBT).GetProperty("CreateDate") != null)
                        {
                            DBItem.GetType().GetProperty("CreateDate").SetValue(DBItem, DateTime.Now);
                        }
                        if (typeof(DBT).GetProperty("CreatedBy") != null)
                        {
                            DBItem.GetType().GetProperty("CreatedBy").SetValue(DBItem, CurrentUser.Domain.ToUpper() + "\\" + CurrentUser.ADUser.ToLower());
                        }
                        ((Table<DBT>)(LocalDB.GetType().GetProperty(this.ContextPropertyName).GetValue(LocalDB))).InsertOnSubmit(DBItem);
                    }

                    if (DBItem == null)
                    {
                        throw new ArgumentException(typeof(DBT).Name + " is invalid.");
                    }

                    foreach (var Property in ModelProperties)
                    {
                        if (Attribute.IsDefined(Property, typeof(LocalProperty)))
                        {
                            WriteModelPropertyToDBOn<DBT, LocalProperty>(Property, Item, DBItem);
                        }
                    }

                    //These should not be alterable
                    if (EmpGeneralIDPropertyName != null)
                        DBItem.GetType().GetProperty(EmpGeneralIDPropertyName).SetValue(DBItem, EmployeeHRMS.CoreEmpGeneralID);
                    if (DBItem.GetType().GetProperty("IsDeleted") == null)
                    {
                        DBItem.GetType().GetProperty("IsActive").SetValue(DBItem, 'Y');
                    }
                    if (typeof(DBT).GetProperty("UpdateDate") != null)
                    {
                        DBItem.GetType().GetProperty("UpdateDate").SetValue(DBItem, DateTime.Now);
                    }
                    if (typeof(DBT).GetProperty("UpdatedBy") != null)
                    {
                        DBItem.GetType().GetProperty("UpdatedBy").SetValue(DBItem, CurrentUser.Domain.ToUpper() + "\\" + CurrentUser.ADUser.ToLower());
                    }

                    if (!((int?)(Item.GetType().GetProperty(PrimaryKey.Name).GetValue(Item))).HasValue)
                    {
                        InsertedItems.Add(DBItem);
                    }
                    else
                    {
                        Log.LogItemChanges<DBT>(LocalDB, (object)DBItem);
                    }
                }
            }
            LocalDB.SubmitChanges();

            Log.LogItemInserts<DBT>(LocalDB, InsertedItems);

            return BuildModelFromEmpMasterID(EmpMasterID, true);
        }

        protected virtual void DeleteDeletedItems<DBT>(ModelCollection<ModelT> Models)
            where DBT : class
        {

            var PrimaryKey = GetModelPrimaryKey();
            var DBPrimaryKey = typeof(ModelT).GetProperty(PrimaryKey.Name).GetCustomAttribute<LocalProperty>(false).ColumnName ?? PrimaryKey.Name;
            var EmpGeneralIDPropertyName = this.GetEmpGeneralIDPropertyName();

            if (CurrentUser.HasPermissionTo(typeof(ModelT), PermissionLevel.DELETE))
            {
                var EmpGeneralID = EmployeeHRMS.CoreEmpGeneralID;
                var ExistingIDs = new List<int>();

                if (Models != null)
                {
                    ExistingIDs = Models.Items
                          .Where(j => ((int?)(j.GetType().GetProperty(PrimaryKey.Name).GetValue(j))).HasValue)
                          .Select(j => (int)j.GetType().GetProperty(PrimaryKey.Name).GetValue(j))
                          .ToList();
                }

                var WhereClause = ExistingIDs.Select(x => DBPrimaryKey + "!=" + x.ToString());
                IQueryable<DBT> DeletedItems;
                if (EmpGeneralIDPropertyName != null)
                {
                    DeletedItems = ((Table<DBT>)(LocalDB.GetType().GetProperty(this.ContextPropertyName).GetValue(LocalDB)))
                                               .Where(EmpGeneralIDPropertyName + "=@0", EmpGeneralID);

                    if (WhereClause.Count() > 0)
                    {
                        DeletedItems = DeletedItems.Where(string.Join(" AND ", WhereClause));
                    }
                }
                else if(WhereClause.Count() > 0)
                {
                    DeletedItems = ((Table<DBT>)(LocalDB.GetType().GetProperty(this.ContextPropertyName).GetValue(LocalDB)))
                                               .Where(string.Join(" AND ", WhereClause));
                }
                else
                {
                    return;
                }
                

                if (this.BuilderSpecification != null)
                {
                    var ItemSubset = (IEnumerable<DBT>)this.BuilderSpecification.GetAllItems(this.EmployeeHRMS.CoreEmpGeneralID);
                    var Where = new List<string>();
                    foreach (var Item in ItemSubset)
                    {
                        Where.Add(DBPrimaryKey +"="+ ((int?)typeof(DBT).GetProperty(DBPrimaryKey).GetValue(Item)).ToString());
                    }

                    if (Where.Count() > 0)
                    {
                        DeletedItems = DeletedItems.Where(string.Join(" OR ", Where));
                    }
                    else
                    {
                        return;
                    }
                }

                foreach (var DeletedItem in DeletedItems)
                {
                    var deleteProperty = DeletedItem.GetType().GetProperty("IsDeleted");
                    if (deleteProperty != null)
                    {
                        if (deleteProperty.PropertyType.IsGenericType &&
                            deleteProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))//.GetType().UnderlyingSystemType == typeof(char))
                        {
                            Type underLyingType = Nullable.GetUnderlyingType(deleteProperty.PropertyType);
                            if(underLyingType==typeof(char))
                                deleteProperty.SetValue(DeletedItem, 'Y');
                            else if (underLyingType == typeof(bool) )
                            {
                                deleteProperty.SetValue(DeletedItem, true);
                            }
                        }
                        else
                        {
                            if(deleteProperty.PropertyType == typeof(bool))
                            {
                                deleteProperty.SetValue(DeletedItem,true);
                            }
                        }
                    }
                    else
                    {
                        DeletedItem.GetType().GetProperty("IsActive").SetValue(DeletedItem, 'N');
                    }
                    Logger Log = new Logger(EmployeeMaster.EmpMasterID);
                    Log.LogItemChanges<DBT>(LocalDB, (object)DeletedItem);
                }

                LocalDB.SubmitChanges();
            }
        }

        protected virtual IEnumerable<ModelT> FillItems<DBT>(bool ClearCache = false)
            where DBT : class
        {
            List<ModelT> Models = new List<ModelT>();
            var EmpGeneralID = this.EmployeeHRMS.CoreEmpGeneralID;
            var EmpGeneralIDPropertyName = this.GetEmpGeneralIDPropertyName();
            var DBType = typeof(DBT);
            IEnumerable<DBT> DBItems = null;

            if (BuilderSpecification != null)
            {
                DBItems = (IEnumerable<DBT>)BuilderSpecification.GetAllItems(EmpGeneralID);
            }
            else
            {
                DBItems = (IEnumerable<DBT>)((Table<DBT>)(LocalDB.GetType()
                                                           .GetProperty(this.ContextPropertyName)
                                                           .GetValue(LocalDB)))
                                            .Where(EmpGeneralIDPropertyName + "=@0 AND IsActive=@1", EmpGeneralID, 'Y');
            }

            if (ClearCache)
            {
                try
                {
                    LocalDB.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, DBItems);
                }
                catch (ArgumentException) { }
            }

            foreach (var Item in DBItems)
            {
                var Model = new ModelT();
                var ModelProperties = Model.GetType().GetProperties();
                foreach (var Property in ModelProperties)
                {
                    if (Attribute.IsDefined(Property, typeof(LocalProperty)))
                    {
                        SetModelPropertyOn<DBT, LocalProperty>(Property, Item, Model);
                    }
                }
                Models.Add(Model);
            }

            return Models;
        }

        private string GetEmpGeneralIDPropertyName()
        {

            if (Attribute.IsDefined(typeof(ModelT), typeof(EmpGeneralIDProperty)))
            {
                var EmpGeneralIDAttribute = typeof(ModelT).GetCustomAttribute<EmpGeneralIDProperty>(false);
                return EmpGeneralIDAttribute.ColumnName;
            }
            return null;
        }

        protected PropertyInfo GetModelPrimaryKey()
        {
            var Properties = typeof(ModelT).GetProperties();

            foreach (var Property in Properties)
            {
                if (Attribute.IsDefined(Property, typeof(KeyAttribute)))
                {
                    return Property;
                }
            }

            return null;
        }
    }
}
