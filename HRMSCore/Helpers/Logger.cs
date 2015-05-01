using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using HRMS.Modules.DBModel.Local;
using System.Data.Linq.Mapping;
using HRMS.Modules.DBModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Local.Models.ViewModels;

namespace Local.Classes.Helpers
{
    public class Logger
    {
        private int EmpMasterID;
        private bool IsSelfService;
        private string ImpersonateUser;

        public Logger(int EmpMasterID)
        {
            this.EmpMasterID = EmpMasterID;
            this.IsSelfService = DBHelper.IsSelfService();
            if (CookieHelper.IsImpersonating)
            {
                HttpCookie impersonateCookie = HttpContext.Current.Request.Cookies[CookieHelper.IMPERSONATE_COOKIE_NAME];
                ImpersonateUser = impersonateCookie.Value;
            }
            else
                ImpersonateUser = null;
        }

        public void LogItemChanges<EntityT>(DataContext Context, object Entity, string RealUser = null)
        {
            var Meta = Context.Mapping.GetTable(typeof(EntityT));
            var ModifiedMembers = Context.GetTable(typeof(EntityT)).GetModifiedMembers(Entity);

            foreach (var Member in ModifiedMembers)
            {
                if (Context is HrmsMasterDataClassesDataContext) {
                    MasterAuditLog Log = new MasterAuditLog()
                    {
                        EmpMasterID = this.EmpMasterID,
                        ItemID = Meta.RowType.IdentityMembers.Count > 0 ? (int)Entity.GetType().GetProperty(Meta.RowType.IdentityMembers[0].Member.Name).GetValue(Entity) : 0,
                        RealUser = RealUser ?? DBHelper.GetCurrentUserContext(true).Domain.ToUpper() + "\\" + DBHelper.GetCurrentUserContext(true).ADUser.ToLower(),
                        Field = Member.Member.Name,
                        Table = Member.Member.ReflectedType.Name,
                        ImpersonatingUser = ImpersonateUser, 
                        IsSelfService = IsSelfService,
                        UpdateDate = DateTime.Now,
                        OldValue = Member.OriginalValue != null ? Member.OriginalValue.ToString() : null,
                        NewValue = Member.CurrentValue != null ? Member.CurrentValue.ToString() : null,
                    };
                    ((HrmsMasterDataClassesDataContext)Context).MasterAuditLog.InsertOnSubmit(Log);
                }
                else if (Context is LocalDataClassesDataContext) {
                    LocalAuditLog Log = new LocalAuditLog()
                    {
                        EmpMasterID = this.EmpMasterID,
                        ItemID = Meta.RowType.IdentityMembers.Count > 0 ? (int)Entity.GetType().GetProperty(Meta.RowType.IdentityMembers[0].Member.Name).GetValue(Entity) : 0,
                        RealUser = DBHelper.GetCurrentUserContext(true).Domain.ToUpper() + "\\" + DBHelper.GetCurrentUserContext(true).ADUser.ToLower(),
                        Field = Member.Member.Name,
                        Table = Member.Member.ReflectedType.Name,
                        ImpersonatingUser = ImpersonateUser, // FIXME
                        IsSelfService = IsSelfService,
                        UpdateDate = DateTime.Now,
                        OldValue = Member.OriginalValue != null ? Member.OriginalValue.ToString() : null,
                        NewValue = Member.CurrentValue != null ? Member.CurrentValue.ToString() : null,
                    };
                    ((LocalDataClassesDataContext)Context).LocalAuditLog.InsertOnSubmit(Log);
                }
            }

            Context.SubmitChanges();
        }

        public void LogItemInsert<EntityT>(DataContext Context, EntityT Entity, string RealUser = null)
        {
            LogItemInserts<EntityT>(Context, new EntityT[] { Entity }, RealUser);
        }

        public void LogItemInserts<EntityT>(DataContext Context, IEnumerable<EntityT> Entities, string RealUser = null)
        {
            var Properties = typeof(EntityT).GetProperties();
            var Meta = Context.Mapping.GetTable(typeof(EntityT));

            foreach (var Entity in Entities)
            {

                foreach (var Property in Properties)
                {
                    if (Attribute.IsDefined(Property, typeof(global::System.Data.Linq.Mapping.ColumnAttribute)))
                    {
                        if (Context is HrmsMasterDataClassesDataContext)
                        {
                            MasterAuditLog Log = new MasterAuditLog()
                            {
                                EmpMasterID = this.EmpMasterID,
                                ItemID = Meta.RowType.IdentityMembers.Count > 0 ? (int)Entity.GetType().GetProperty(Meta.RowType.IdentityMembers[0].Member.Name).GetValue(Entity) : 0,
                                RealUser = RealUser ?? DBHelper.GetCurrentUserContext(true).Domain.ToUpper() + "\\" + DBHelper.GetCurrentUserContext(true).ADUser.ToLower(),
                                Field = Property.Name,
                                Table = typeof(EntityT).Name,
                                ImpersonatingUser = ImpersonateUser,
                                IsSelfService = IsSelfService,
                                UpdateDate = DateTime.Now,
                                OldValue = "[ NewItem ]",
                                NewValue = Property.GetValue(Entity) != null ? Property.GetValue(Entity).ToString() : null,
                            };
                            ((HrmsMasterDataClassesDataContext)Context).MasterAuditLog.InsertOnSubmit(Log);
                        }
                        else if (Context is LocalDataClassesDataContext)
                        {
                            LocalAuditLog Log = new LocalAuditLog()
                            {
                                EmpMasterID = this.EmpMasterID,
                                ItemID = Meta.RowType.IdentityMembers.Count > 0 ? (int)Entity.GetType().GetProperty(Meta.RowType.IdentityMembers[0].Member.Name).GetValue(Entity) : 0,
                                RealUser = DBHelper.GetCurrentUserContext(true).Domain.ToUpper() + "\\" + DBHelper.GetCurrentUserContext(true).ADUser.ToLower(),
                                Field = Property.Name,
                                Table = typeof(EntityT).Name,
                                ImpersonatingUser = ImpersonateUser, 
                                IsSelfService = IsSelfService,
                                UpdateDate = DateTime.Now,
                                OldValue = "[ NewItem ]",
                                NewValue = Property.GetValue(Entity) != null ? Property.GetValue(Entity).ToString() : null,
                            };
                            ((LocalDataClassesDataContext)Context).LocalAuditLog.InsertOnSubmit(Log);
                        }
                    }
                }
            }

            Context.SubmitChanges();
        }

        public void LogRequestForShowingAdditionalInfo(LocalDataClassesDataContext context, ShowHiddenFieldsViewModel model)
        {
            LocalAuditLog Log = new LocalAuditLog()
            {
                EmpMasterID = model.EmpMasterId,
                Field = model.Action,
                Table = model.Tab,
                ImpersonatingUser = ImpersonateUser,
                IsSelfService = IsSelfService,
                RealUser = DBHelper.GetCurrentUserContext(true).Domain.ToUpper() + "\\" + DBHelper.GetCurrentUserContext(true).ADUser.ToLower(),
                UpdateDate= DateTime.Now
            };
            context.LocalAuditLog.InsertOnSubmit(Log);
            context.SubmitChanges();
        }
    }
}
