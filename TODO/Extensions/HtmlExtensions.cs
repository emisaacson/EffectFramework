using HRMS.Core.Helpers;
using HRMS.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using HRMS.Core.Security;
using HRMS.Core.Attributes;
using System.Web.Helpers;
using System.Web.Routing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.UI.WebControls.Expressions;
using System.Reflection;

namespace HRMS.Core.Extensions
{
    public static class HtmlExtensions
    {
        public static bool IsDebug(this HtmlHelper htmlHelper)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public static bool IsStaging(this HtmlHelper htmlHelper)
        {
#if STAGING
            return true;
#else
            return false;
#endif

        }
        public static bool EntityShouldSee<TModel, TValue>(this HtmlHelper<TModel> Html, Expression<Func<TModel, TValue>> Exp)
        {
            var Metadata = ModelMetadata.FromLambdaExpression(Exp, Html.ViewData);

            if (Metadata.AdditionalValues.ContainsKey(EntityRestriction.MetaKey))
            {
                var CurrentDatabase = DBHelper.GetCurrentDatabase();
                if (!((List<string>)(Metadata.AdditionalValues[EntityRestriction.MetaKey])).Select(x => x.ToLower()).Contains(CurrentDatabase.ToLower()))
                {
                    return false;
                }
            }

            return true;
        }
        public static MvcHtmlString FormFieldViewFor<TModel, TValue>(
            this HtmlHelper<TModel> Html,
            Expression<Func<TModel, TValue>> Exp,
            bool IsBackboneTemplate = false,
            bool ShowEmptyText = false,
            bool ForceReadOnly = false,
            bool SmallerLabel = false,
            string CustomEmptyText = null
        )
        {

            var User = DBHelper.GetCurrentUserContext();

            var Metadata = ModelMetadata.FromLambdaExpression(Exp, Html.ViewData);
            //var RequiredPermission = (string)Metadata.AdditionalValues[RequirePermission.MetaKey];
            PropertyInfo proInfo = null;
            if(Exp.Body is MemberExpression)
            {
                var body = (MemberExpression)Exp.Body;
                proInfo = (PropertyInfo)body.Member;
            }
            string LabelWidth = "4";
            string ValueWidth = "8";

            if (SmallerLabel)
            {
                LabelWidth = "2";
                ValueWidth = "10";
            }

            if (Metadata.AdditionalValues.ContainsKey(EntityRestriction.MetaKey))
            {
                var CurrentDatabase = DBHelper.GetCurrentDatabase();
                if (!((List<string>)(Metadata.AdditionalValues[EntityRestriction.MetaKey])).Select(x => x.ToLower()).Contains(CurrentDatabase.ToLower()))
                {
                    if (Metadata.IsRequired)
                    {
                        return HRMSDisplayFor(Html, Exp, IsBackboneTemplate, ShowEmptyText, ForceReadOnly, CustomEmptyText, true);
                    }
                    else
                    {
                        return new MvcHtmlString("");
                    }
                }
            }

            if (DBHelper.IsSelfService() == true)
            {
                var SSFieldInfo = ModelPropertyInfoHelper.GetSSFieldInfo(proInfo);
                if(SSFieldInfo.IsAvailableInSS==false)
                {
                    return new MvcHtmlString("");
                }
                else if(SSFieldInfo.IsReadOnlyInSS)
                {
                    ForceReadOnly = true;
                }
            }

            string Description = ModelPropertyInfoHelper.GetPropertyDescription(proInfo, Metadata);
            
            if (proInfo != null && User.HasPermissionTo(proInfo, PermissionLevel.READ))
            {
                return new MvcHtmlString(
                    @"<div class='profile-info-row row'>
                          <div class='col-sm-" + LabelWidth + @"'>
                              <div class='profile-info-name'>" + Html.HRMSLabelFor(Exp).ToString() + @"</div>" +
                             (Description.Length > 0 ? @"<div style=""text-align: right;""> <small><span class = ""text-muted"">" + Description + @"</span> </small></div>" : "") +
                          @"</div>
                          <div class='col-sm-" + ValueWidth + @"'>
                            <div class='profile-info-value'>" +
                                Html.SecurityTrimmedHRMSDisplayFor(Exp, IsBackboneTemplate, ShowEmptyText, ForceReadOnly, CustomEmptyText).ToString() +
                            @"</div>
                          </div>
                      </div>");
            }
            else if (Metadata.IsRequired)
            {
                return HRMSDisplayFor(Html, Exp, IsBackboneTemplate, ShowEmptyText, ForceReadOnly, CustomEmptyText, true);
            }
            return new MvcHtmlString("");
        }
        public static MvcHtmlString SecurityTrimmedHRMSDisplayFor<TModel, TValue>(
            this HtmlHelper<TModel> Html,
            Expression<Func<TModel, TValue>> Exp,
            bool IsBackboneTemplate = false,
            bool ShowEmptyText = false,
            bool ForceReadOnly = false,
            string CustomEmptyText = null

        )
        {
            var User = DBHelper.GetCurrentUserContext();
            
            var Metadata = ModelMetadata.FromLambdaExpression(Exp, Html.ViewData);
            //var RequiredPermission = (string)Metadata.AdditionalValues[RequirePermission.MetaKey];
            PropertyInfo proInfo = null;
            if (Exp.Body is MemberExpression)
            {
                var body = (MemberExpression)Exp.Body;
                proInfo = (PropertyInfo)body.Member;
            }
            if (Metadata.AdditionalValues.ContainsKey(EntityRestriction.MetaKey))
            {
                var CurrentDatabase = DBHelper.GetCurrentDatabase();
                if (!((List<string>)(Metadata.AdditionalValues[EntityRestriction.MetaKey])).Select(x => x.ToLower()).Contains(CurrentDatabase.ToLower()))
                {
                    if (Metadata.IsRequired)
                    {
                        return HRMSDisplayFor(Html, Exp, IsBackboneTemplate, ShowEmptyText, ForceReadOnly, CustomEmptyText, true);
                    }
                    else
                    {
                        return new MvcHtmlString("");
                    }
                }
            }
            if (proInfo != null && User.HasPermissionTo(proInfo, PermissionLevel.READ))
            {

                return HRMSDisplayFor(Html, Exp, IsBackboneTemplate, ShowEmptyText, ForceReadOnly, CustomEmptyText);
            }
            else if (Metadata.IsRequired)
            {
                return HRMSDisplayFor(Html, Exp, IsBackboneTemplate, ShowEmptyText, ForceReadOnly, CustomEmptyText, true);
            }

            return new MvcHtmlString("");
        }

             public static MvcHtmlString HRMSLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression,bool isRequired = false)
        {
            return HRMSLabelFor(html, expression, new { }, isRequired);
        }

        public static MvcHtmlString HRMSLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes, bool isRequired = false)
        {
            return HRMSLabelFor(html, expression, new RouteValueDictionary(htmlAttributes),isRequired);
        }
        public static MvcHtmlString HRMSLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes, bool isRequired=false)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            PropertyInfo proInfo = null;
            if (expression.Body is MemberExpression)
            {
                var body = (MemberExpression)expression.Body;
                proInfo = (PropertyInfo)body.Member;
            }

            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            if (DBHelper.IsSelfService() == true)
            {
                var SSFieldInfo = ModelPropertyInfoHelper.GetSSFieldInfo(proInfo);
                if (SSFieldInfo.IsAvailableInSS == false)
                {
                    return new MvcHtmlString("");
                }
            }

            TagBuilder tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            TagBuilder span = new TagBuilder("span");
            span.SetInnerText(labelText);

            // assign <span> to <label> inner html
            tag.InnerHtml = span.ToString(TagRenderMode.Normal);

            if (metadata.IsRequired || isRequired)
            {
                if (tag.Attributes.ContainsKey("class"))
                {
                    tag.Attributes["class"] += " required";
                }
                else
                {
                    tag.Attributes.Add("class", "required");
                }
            }

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        public static bool CurrentUserHasPermissionTo<TModel, TValue>(this HtmlHelper<TModel> Html, PermissionLevel PermissionLevel, Expression<Func<TModel, TValue>> Exp)
        {
            var User = DBHelper.GetCurrentUserContext();

            var Metadata = ModelMetadata.FromLambdaExpression(Exp, Html.ViewData);
            var RequiredPermission = (string)Metadata.AdditionalValues.ValueOrDefault(RequirePermission.MetaKey);
            PropertyInfo proInfo = null;
            if (Exp.Body is MemberExpression)
            {
                var body = (MemberExpression)Exp.Body;
                proInfo = (PropertyInfo)body.Member;
            }
            return User.HasPermissionTo(proInfo, PermissionLevel);
        }

        private static MvcHtmlString HRMSDisplayFor<TModel, TValue>(
                HtmlHelper<TModel> Html,
                Expression<Func<TModel, TValue>> Exp,
                bool IsBackboneTemplate,
                bool ShowEmptyText,
                bool ForceReadOnly,
                string CustomEmptyText,
                bool ForceHidden = false
            )
        {
            var User = DBHelper.GetCurrentUserContext();

            var Metadata = ModelMetadata.FromLambdaExpression(Exp, Html.ViewData);
            var Choices = (KeyValuePair<string, string>[])Metadata.AdditionalValues.ValueOrDefault(HRMS.Core.Attributes.Choices.MetaKey);
            //var RequiredPermission = (string)Metadata.AdditionalValues.ValueOrDefault(RequirePermission.MetaKey);
            PropertyInfo proInfo = null;
            if (Exp.Body is MemberExpression)
            {
                var body = (MemberExpression)Exp.Body;
                proInfo = (PropertyInfo)body.Member;
            }
            bool IsVisible = true;
            string MarkForEncriptedFields = String.Empty;
            //var isVisbleObj = Metadata.AdditionalValues.ValueOrDefault(Encrypted.MetaKey);
            //if (isVisbleObj != null)
            //    IsVisible = (bool)isVisbleObj;

            ReferenceAttribute Ref = null;
            Ref = (MasterReference)Metadata.AdditionalValues.ValueOrDefault(MasterReference.MetaKey);
            if (Ref == null)
            {
                Ref = (LocalReference)Metadata.AdditionalValues.ValueOrDefault(LocalReference.MetaKey);
            }

            bool IsTextarea = Metadata.ContainerType
                                      .GetProperty(Metadata.PropertyName)
                                      .GetCustomAttributes(false)
                                      .OfType<Textarea>()
                                      .FirstOrDefault() != null;

            bool IsNumeric = Metadata.ContainerType
                                      .GetProperty(Metadata.PropertyName)
                                      .GetCustomAttributes(false)
                                      .OfType<Numeric>()
                                      .FirstOrDefault() != null;

            bool IsMultichoice = Metadata.ContainerType
                                      .GetProperty(Metadata.PropertyName)
                                      .GetCustomAttributes(false)
                                      .OfType<MultiChoice>()
                                      .FirstOrDefault() != null;

            bool IsEmailAddress = Metadata.ContainerType
                                      .GetProperty(Metadata.PropertyName)
                                      .GetCustomAttributes(false)
                                      .OfType<EmailAddressAttribute>()
                                      .FirstOrDefault() != null;

            bool IsAlphanumeric = Metadata.ContainerType
                          .GetProperty(Metadata.PropertyName)
                          .GetCustomAttributes(false)
                          .OfType<Alphanumeric>()
                          .FirstOrDefault() != null;

            bool IsPassword = Metadata.ContainerType
                                      .GetProperty(Metadata.PropertyName)
                                      .GetCustomAttributes(false)
                                      .OfType<PasswordInput>()
                                      .FirstOrDefault() != null;

            bool IsHidden = ForceHidden || Metadata.ContainerType
                                      .GetProperty(Metadata.PropertyName)
                                      .GetCustomAttributes(false)
                                      .OfType<Hidden>()
                                      .FirstOrDefault() != null;

            bool IsCurrency = Metadata.ContainerType
                                      .GetProperty(Metadata.PropertyName)
                                      .GetCustomAttributes(false)
                                      .OfType<CurrencyField>()
                                      .FirstOrDefault() != null;

            bool IsEncrypted = Metadata.ContainerType
                                      .GetProperty(Metadata.PropertyName)
                                      .GetCustomAttributes(false)
                                      .OfType<Encrypted>()
                                      .FirstOrDefault() != null;

            if (IsEncrypted )
            {
                if(String.IsNullOrEmpty(CookieHelper.GetEncriptionKeyFromCookie()))
                {
                    return new MvcHtmlString("");
                }
                else
                {
                    IsVisible = false;
                }
            }



            var IsWritable = false;
            //if (DBHelper.IsSelfService() == true)
            //{
            //    var SSFieldInfo = ModelPropertyInfoHelper.GetSSFieldInfo(proInfo);
            //    if (SSFieldInfo.IsAvailableInSS == false)
            //    {
            //        return new MvcHtmlString("");
            //    }
            //    else if (SSFieldInfo.IsReadOnlyInSS)
            //    {
            //        ForceReadOnly = true;
            //    }
            //}

            if ((proInfo != null && User.HasPermissionTo(proInfo, PermissionLevel.CREATE)) && !Metadata.IsReadOnly && !ForceReadOnly)
            {
                IsWritable = true;
            }

            if(proInfo!=null && !User.HasPermissionTo(proInfo, PermissionLevel.READ))
            {
                return new MvcHtmlString(string.Empty);
            }


            // Hidden Fields
            if (IsHidden)
            {
                return Html.DisplayFor(Exp, "Hidden");
            }

            // References
            if (Ref != null)
            {
                var Select2Choices = (new string[] { "" }).Select(s => new { id = s, value = s, text = " " }).ToList();

                foreach (var Item in Ref.Items)
                {
                    Select2Choices.Add(new
                    {
                        id = Ref.PropertyType.GetProperty(Ref.KeyProperty).GetValue(Item).ToString(),
                        value = Ref.PropertyType.GetProperty(Ref.KeyProperty).GetValue(Item).ToString(),
                        text = Ref.PropertyType.GetProperty(Ref.ValueProperty).GetValue(Item).ToString()
                    });
                }

                Select2Choices = Select2Choices.OrderBy(x => x.text).ToList();

                if (Metadata.ModelType == typeof(string))
                {
                    return Html.DisplayFor(Exp, "Choice", new
                    {
                        IsWritable,
                        JsonChoices = Json.Encode(Select2Choices.Select(x => new { id = x.text, value = x.text, text = x.text })),
                        Choices = Select2Choices.Select(s => new KeyValuePair<string, string>(s.text, s.text)).ToArray(),
                        IsBackboneTemplate,
                        ShowEmptyText,
                        CustomEmptyText
                    });
                }
                else
                {
                    if (IsMultichoice)
                    {
                        return Html.DisplayFor(Exp, "MultiChoiceInt", new
                        {
                            IsWritable,
                            JsonChoices = Json.Encode(Select2Choices),
                            Choices = Select2Choices.Select(s => new KeyValuePair<string, string>(s.id, s.text)).ToArray(),
                            IsBackboneTemplate,
                            ShowEmptyText,
                            CustomEmptyText
                        });
                    }
                    else
                    {
                        return Html.DisplayFor(Exp, "ChoiceInt", new
                        {
                            IsWritable,
                            JsonChoices = Json.Encode(Select2Choices),
                            Choices = Select2Choices.Select(s => new KeyValuePair<string, string>(s.id, s.text)).ToArray(),
                            IsBackboneTemplate,
                            ShowEmptyText,
                            CustomEmptyText
                        });
                    }
                }
            }

            //Choices
            else if (Choices != null)
            {
                var Select2Choices = Choices.Select(s => new { id = s.Key, value = s.Key, text = s.Value }).ToList();
                Select2Choices.Insert(0, new { id = "", value = "", text = " " });

                if (Metadata.ModelType == typeof(string))
                {
                    return Html.DisplayFor(Exp, "Choice", new {
                        IsWritable,
                        JsonChoices = Json.Encode(Select2Choices),
                        Choices,
                        IsBackboneTemplate,
                        ShowEmptyText,
                        CustomEmptyText
                    });
                }
                else if (Metadata.ModelType == typeof(char) || Metadata.ModelType == typeof(char?))
                {
                    return Html.DisplayFor(Exp, "ChoiceChar", new {
                        IsWritable,
                        JsonChoices = Json.Encode(Select2Choices),
                        Choices,
                        IsBackboneTemplate,
                        ShowEmptyText,
                        CustomEmptyText
                    });
                }
                else if (Metadata.ModelType == typeof(int) || Metadata.ModelType == typeof(int?))
                {
                    return Html.DisplayFor(Exp, "ChoiceInt", new
                    {
                        IsWritable,
                        JsonChoices = Json.Encode(Select2Choices),
                        Choices,
                        IsBackboneTemplate,
                        ShowEmptyText,
                        CustomEmptyText
                    });
                }
                else if (Metadata.ModelType == typeof(bool) || Metadata.ModelType == typeof(bool?))
                {
                    return Html.DisplayFor(Exp, "ChoiceBool", new
                    {
                        IsWritable,
                        JsonChoices = Json.Encode(Select2Choices),
                        Choices,
                        IsBackboneTemplate,
                        ShowEmptyText,
                        CustomEmptyText
                    });
                }
                else
                {
                    throw new ArgumentException("Not a valid choice property.");
                }
            }

            // Text
            else if (IsTextarea) {
                return Html.DisplayFor(Exp, "Textarea", new
                {
                    IsWritable,
                    IsBackboneTemplate,
                    ShowEmptyText,
                    CustomEmptyText
                });
            }

            // Currency Field
            else if (IsCurrency)
            {
                return Html.DisplayFor(Exp, "Currency", new
                {
                    IsWritable,
                    IsBackboneTemplate,
                    ShowEmptyText,
                    isVisible = IsVisible,
                    CustomEmptyText
                });
            }

            // Numeric text
            else if (IsNumeric)
            {
                return Html.DisplayFor(Exp, "Numeric", new
                {
                    IsWritable,
                    IsBackboneTemplate,
                    ShowEmptyText,
                    CustomEmptyText
                });
            }

            // Email address
            else if (IsEmailAddress)
            {
                return Html.DisplayFor(Exp, "String", new
                {
                    IsWritable,
                    IsBackboneTemplate,
                    ShowEmptyText,
                    IsEmailAddress,
                    isVisible = IsVisible,
                    CustomEmptyText
                });
            }

            // Alphanumeric
            else if (IsAlphanumeric)
            {
                return Html.DisplayFor(Exp, "String", new
                {
                    IsWritable,
                    IsBackboneTemplate,
                    ShowEmptyText,
                    IsAlphanumeric,
                    isVisible = IsVisible,
                    CustomEmptyText
                });
            }

            // Password
            else if (IsPassword)
            {
                return Html.DisplayFor(Exp, "String", new
                {
                    IsWritable,
                    IsBackboneTemplate,
                    ShowEmptyText,
                    CustomEmptyText,
                    isVisible = IsVisible,
                    IsPassword
                });
            }

            // Default (by raw data type)
            else {
                return Html.DisplayFor(Exp, new {
                    IsWritable,
                    IsBackboneTemplate,
                    ShowEmptyText,
                    isVisible = IsVisible,
                    CustomEmptyText
                });
            }
        }
    }

}