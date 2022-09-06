using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Collections;
using System.Globalization;
using System.Text;
using MainOffice.App_LocalResources;
using System.Web.UI.WebControls;
using System.Dynamic;
using System.Web.Routing;
using System.Web.Mvc.Ajax;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using MainOffice.Functions;

namespace MainOffice.Helpers
{
    public static class MvcHtmlHelpers
    {
        #region DisplayShortNameFor
        /// <summary>Получает отображаемое имя для модели.</summary>
        /// <returns>Отображаемое имя для модели.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий отображаемое имя.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString DisplayShortNameFor<TModel, TValue>(
          this HtmlHelper<IEnumerable<TModel>> html,
          Expression<Func<TModel, TValue>> expression)
        {
            return html.DisplayShortNameForInternal<TModel, TValue>(expression, (ModelMetadataProvider)null);
        }

        internal static MvcHtmlString DisplayShortNameForInternal<TModel, TValue>(
          this HtmlHelper<IEnumerable<TModel>> html,
          Expression<Func<TModel, TValue>> expression,
          ModelMetadataProvider metadataProvider)
        {
            return MvcHtmlHelpers.DisplayShortNameHelper(ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, new ViewDataDictionary<TModel>()), ExpressionHelper.GetExpressionText((LambdaExpression)expression));
        }

        /// <summary>Получает отображаемое имя для модели.</summary>
        /// <returns>Отображаемое имя для модели.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий отображаемое имя.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString DisplayShortNameFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression)
        {
            return html.DisplayShortNameForInternal<TModel, TValue>(expression, (ModelMetadataProvider)null);
        }

        internal static MvcHtmlString DisplayShortNameForInternal<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression,
          ModelMetadataProvider metadataProvider)
        {
            return MvcHtmlHelpers.DisplayShortNameHelper(ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, html.ViewData), ExpressionHelper.GetExpressionText((LambdaExpression)expression));
        }

       
        internal static MvcHtmlString DisplayShortNameHelper(
          ModelMetadata metadata,
          string htmlFieldName)
        {
            string s = metadata.ShortDisplayName;
            if (s == null)
            {
                s = metadata.DisplayName;
                if (s == null)
                {
                    string propertyName = metadata.PropertyName;
                    if (propertyName == null)
                        s = ((IEnumerable<string>)htmlFieldName.Split('.')).Last<string>();
                    else
                        s = propertyName;
                }
            }
            return new MvcHtmlString(HttpUtility.HtmlEncode(s));
        }
        #endregion
        #region DisplayCustomForModel
        /// <summary>Получает отображаемое имя для модели.</summary>
        /// <returns>Отображаемое имя для модели.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        public static MvcHtmlString DisplayCustomForModel<TModel>(this HtmlHelper<IEnumerable<TModel>> html, string ToDisplay)
        {
            return DisplayCustomForModelHelper(ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TModel)), ToDisplay);
        }
        /// <summary>Получает отображаемое имя для модели.</summary>
        /// <returns>Отображаемое имя для модели.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        public static MvcHtmlString DisplayCustomForModel<TModel>(this HtmlHelper<TModel> html, string ToDisplay)
        {
            return DisplayCustomForModelHelper(html.ViewData.ModelMetadata, ToDisplay);
        }


        internal static MvcHtmlString DisplayCustomForModelHelper(
          ModelMetadata metadata,
          string ToDisplay)
        {
            string s = null;
            switch (ToDisplay)
            {
                case "Name":
                    s = metadata.DisplayName;
                    break;
                case "ShortName":
                    s = metadata.ShortDisplayName;
                    break;
                case "GroupName":
                    if (metadata.AdditionalValues.ContainsKey("List"))
                    s = metadata.AdditionalValues["List"].ToString();
                    break;
                default:
                    s = "Значения ToDisplay могут быть только \"Name\", \"ShortName\", \"GroupName\"";
                    break;
            }
            if (s == null)
            {
                s = "В метаданных " + ToDisplay + " имеет значение null";
            }
            return new MvcHtmlString(HttpUtility.HtmlEncode(s));
        }
        #endregion
        #region ButtonsInternal

        private static MvcHtmlString ButtonsInternal<TContainer, TValue>(
      this HtmlHelper htmlHelper,
      ModelMetadata metadata,
      string templateName,
      string htmlFieldName,
      DataBoundControlMode mode,
      object additionalViewData)
        {
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            if (string.IsNullOrEmpty(fullHtmlFieldName))
                throw new ArgumentException(GlobalRes.NullOrEmpty, nameof(htmlFieldName));
            StringBuilder stringBuilder = new StringBuilder();
            TagBuilder tagBuilder = new TagBuilder("div");
            tagBuilder.MergeAttribute("class", "form-group");
            stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.StartTag));


            //< div class="btn-group btn-group-toggle btn-group-sm special" id="HireDate" data-toggle="buttons">
            tagBuilder = new TagBuilder("div");
            tagBuilder.MergeAttribute("class", "btn-group btn-group-toggle btn-group-sm special");
            tagBuilder.MergeAttribute("data-toggle", "buttons");
            
            stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.StartTag));
            //< label class="btn btn-outline-secondary active">
            
            
            int selected = (int)metadata.Model;
            for (int i = 0; i < 3; i++)
            {
                tagBuilder = new TagBuilder("label");
                if (i == selected)
                    tagBuilder.MergeAttribute("class", "btn btn-outline-secondary active");
                else
                    tagBuilder.MergeAttribute("class", "btn btn-outline-secondary");
                stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.StartTag));
                //< input type = "radio" name = "optionsHireDate" id = "option1" checked>
                tagBuilder = new TagBuilder("input");
                tagBuilder.MergeAttribute("type", "radio");
                tagBuilder.MergeAttribute("name", htmlFieldName);
                if (i == selected)
                    tagBuilder.MergeAttribute("checked", "");
                switch (i)
                {
                    case 0:
                        tagBuilder.SetInnerText(GlobalRes.All);
                        tagBuilder.MergeAttribute("value", "0");
                        break;
                    case 1:
                        tagBuilder.SetInnerText(GlobalRes.HasValue);
                        tagBuilder.MergeAttribute("value", "1");
                        break;
                    case 2:
                        tagBuilder.SetInnerText(GlobalRes.Empty);
                        tagBuilder.MergeAttribute("value", "2");
                        break;
                }
                stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.Normal));

                tagBuilder = new TagBuilder("label");
                stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.EndTag));
            }
            tagBuilder = new TagBuilder("div");
            stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.EndTag));
            stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.EndTag));

            return MvcHtmlString.Create(stringBuilder.ToString());

        }
       
     
        private static int GetCheckedButton(this HtmlHelper htmlHelper, string name)
        {
            object obj = (object)null;
            int result = 0;
            if (htmlHelper.ViewData != null && !string.IsNullOrEmpty(name))
                obj = htmlHelper.ViewData.Eval(name);
            if (obj == null)
                return 0;
            result = Convert.ToInt32(obj);
            if (result != 1 & result != 2)
                return 0;

            return result;
        }

        #endregion
        #region ButtonsFor
        /// <summary>Возвращает HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением <see cref="T:System.Linq.Expressions.Expression" />.</summary>
        /// <returns>HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString ButtonsFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression)
        {
            return html.ButtonsHelper<TModel, TValue>(expression, (string)null, (string)null, DataBoundControlMode.Edit, (object)null);
        }

        /// <summary>Возвращает HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением, используя дополнительные данные представления.</summary>
        /// <returns>HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="additionalViewData">Анонимный объект, который может содержать дополнительные данные представления, добавляемые в экземпляр <see cref="T:System.Web.Mvc.ViewDataDictionary`1" />, созданный для шаблона.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString ButtonsFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression,
          object additionalViewData)
        {
            return html.ButtonsHelper<TModel, TValue>(expression, (string)null, (string)null, DataBoundControlMode.Edit, additionalViewData);
        }

        /// <summary>Возвращает HTML-элемент input для каждого свойства объекта, представленного выражением <see cref="T:System.Linq.Expressions.Expression" />, используя указанный шаблон.</summary>
        /// <returns>HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="templateName">Имя шаблона, используемого для отображения объекта.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString ButtonsFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression,
          string templateName)
        {
            return html.ButtonsHelper<TModel, TValue>(expression, templateName, (string)null, DataBoundControlMode.Edit, (object)null);
        }

        /// <summary>Возвращает HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением, используя указанный шаблон и дополнительные данные представления.</summary>
        /// <returns>HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="templateName">Имя шаблона, используемого для отображения объекта.</param>
        /// <param name="additionalViewData">Анонимный объект, который может содержать дополнительные данные представления, добавляемые в экземпляр <see cref="T:System.Web.Mvc.ViewDataDictionary`1" />, созданный для шаблона.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString ButtonsFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression,
          string templateName,
          object additionalViewData)
        {
            return html.ButtonsHelper<TModel, TValue>(expression, templateName, (string)null, DataBoundControlMode.Edit, additionalViewData);
        }

        /// <summary>Возвращает HTML-элемент input для каждого свойства объекта, представленного выражением <see cref="T:System.Linq.Expressions.Expression" />, используя указанные шаблон и имя поля HTML.</summary>
        /// <returns>HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="templateName">Имя шаблона, используемого для отображения объекта.</param>
        /// <param name="htmlFieldName">Строка, которая используется для устранения неоднозначности имен HTML-элементов ввода (input), которые отображаются для свойств с одинаковыми именами.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString ButtonsFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression,
          string templateName,
          string htmlFieldName)
        {
            return html.ButtonsHelper<TModel, TValue>(expression, templateName, htmlFieldName, DataBoundControlMode.Edit, (object)null);
        }

        /// <summary>Возвращает HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением, используя указанный шаблон, имя поля HTML и дополнительные данные представления.</summary>
        /// <returns>HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="templateName">Имя шаблона, используемого для отображения объекта.</param>
        /// <param name="htmlFieldName">Строка, которая используется для устранения неоднозначности имен HTML-элементов ввода (input), которые отображаются для свойств с одинаковыми именами.</param>
        /// <param name="additionalViewData">Анонимный объект, который может содержать дополнительные данные представления, добавляемые в экземпляр <see cref="T:System.Web.Mvc.ViewDataDictionary`1" />, созданный для шаблона.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString ButtonsFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression,
          string templateName,
          string htmlFieldName,
          object additionalViewData)
        {
            return html.ButtonsHelper<TModel, TValue>(expression, templateName, htmlFieldName, DataBoundControlMode.Edit, additionalViewData);
        }

        internal static MvcHtmlString ButtonsHelper<TContainer, TValue>(
      this HtmlHelper<TContainer> html,
      Expression<Func<TContainer, TValue>> expression,
      string templateName,
      string htmlFieldName,
      DataBoundControlMode mode,
      object additionalViewData)
        {
            return html.ButtonsInternal<TContainer, TValue>(ModelMetadata.FromLambdaExpression<TContainer, TValue>(expression, html.ViewData), templateName, htmlFieldName ?? ExpressionHelper.GetExpressionText((LambdaExpression)expression), mode, additionalViewData);
        }




        #endregion
        #region ValidationEditorFor

        /// <summary>Возвращает HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением <see cref="T:System.Linq.Expressions.Expression" />.</summary>
        /// <returns>HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString ValidationEditorFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression)
        {
            
            return html.ValidationEditorHelper<TModel, TValue>(expression, null, null, null);
        }

        /// <summary>Возвращает HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением, используя дополнительные данные представления.</summary>
        /// <returns>HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="additionalViewData">Анонимный объект, который может содержать дополнительные данные представления, добавляемые в экземпляр <see cref="T:System.Web.Mvc.ViewDataDictionary`1" />, созданный для шаблона.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString ValidationEditorFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression,
          object additionalViewData)
        {
            return html.ValidationEditorHelper<TModel, TValue>(expression, null, null, additionalViewData);
        }

        /// <summary>Возвращает HTML-элемент input для каждого свойства объекта, представленного выражением <see cref="T:System.Linq.Expressions.Expression" />, используя указанный шаблон.</summary>
        /// <returns>HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="templateName">Имя шаблона, используемого для отображения объекта.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString ValidationEditorFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression,
          string templateName)
        {
            return html.ValidationEditorHelper<TModel, TValue>(expression, templateName, null, null);
        }

        /// <summary>Возвращает HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением, используя указанный шаблон и дополнительные данные представления.</summary>
        /// <returns>HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="templateName">Имя шаблона, используемого для отображения объекта.</param>
        /// <param name="additionalViewData">Анонимный объект, который может содержать дополнительные данные представления, добавляемые в экземпляр <see cref="T:System.Web.Mvc.ViewDataDictionary`1" />, созданный для шаблона.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString ValidationEditorFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression,
          string templateName,
          object additionalViewData)
        {
            return html.ValidationEditorHelper<TModel, TValue>(expression, templateName, null, additionalViewData);
        }

        /// <summary>Возвращает HTML-элемент input для каждого свойства объекта, представленного выражением <see cref="T:System.Linq.Expressions.Expression" />, используя указанные шаблон и имя поля HTML.</summary>
        /// <returns>HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="templateName">Имя шаблона, используемого для отображения объекта.</param>
        /// <param name="htmlFieldName">Строка, которая используется для устранения неоднозначности имен HTML-элементов ввода (input), которые отображаются для свойств с одинаковыми именами.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString ValidationEditorFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression,
          string templateName,
          string htmlFieldName)
        {
            return html.ValidationEditorHelper<TModel, TValue>(expression, templateName, htmlFieldName, null);
        }

        /// <summary>Возвращает HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением, используя указанный шаблон, имя поля HTML и дополнительные данные представления.</summary>
        /// <returns>HTML-элемент ввода (input) для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="html">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="templateName">Имя шаблона, используемого для отображения объекта.</param>
        /// <param name="htmlFieldName">Строка, которая используется для устранения неоднозначности имен HTML-элементов ввода (input), которые отображаются для свойств с одинаковыми именами.</param>
        /// <param name="additionalViewData">Анонимный объект, который может содержать дополнительные данные представления, добавляемые в экземпляр <see cref="T:System.Web.Mvc.ViewDataDictionary`1" />, созданный для шаблона.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        public static MvcHtmlString ValidationEditorFor<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression,
          string templateName,
          string htmlFieldName,
          object additionalViewData)
        {
            if (additionalViewData != null)
            {
                IDictionary<string, object> addViewData = HtmlHelper.ObjectToDictionary(additionalViewData);

                //foreach (KeyValuePair<string, object> keyValuePair in HtmlHelper.ObjectToDictionary(additionalViewData)) ;

            }
            return html.ValidationEditorHelper<TModel, TValue>(expression, templateName, htmlFieldName, additionalViewData);
        }

        internal static MvcHtmlString ValidationEditorHelper<TModel, TValue>(
          this HtmlHelper<TModel> html,
          Expression<Func<TModel, TValue>> expression,
          string templateName,
          string htmlFieldName,
          object additionalViewData)
        {
            string validation = html.ValidationCheck(ExpressionHelper.GetExpressionText((LambdaExpression)expression));
            if (validation != "")
            {
                additionalViewData = EditAdditionalViewData(validation, additionalViewData);
            }
            return html.EditorFor<TModel, TValue>(expression, templateName, htmlFieldName, additionalViewData);
        }

        internal static object EditAdditionalViewData(string validation, object additionalViewData)
        {
            if (additionalViewData != null)
            {
                IDictionary<string, object> viewData = HtmlHelper.ObjectToDictionary(additionalViewData);

                Functions.ObjectExtender extender = new Functions.ObjectExtender();
                object htmlAttributes = new { htmlAttributes = extender.Extend(new { @class = validation }, viewData["htmlAttributes"]) };
                additionalViewData = extender.Extend(htmlAttributes, additionalViewData);
            }
            else
            {
                additionalViewData = new { htmlAttributes = new { @class = validation } };
            }
            return additionalViewData;
        }

        internal static string ValidationCheck<TModel>(
          this HtmlHelper<TModel> html,
          string expression)
        {
            string validation = "";
            string fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expression);
            FormContext clientValidation = html.ViewContext.GetFormContextForClientValidation();
            if (!html.ViewData.ModelState.ContainsKey(fullHtmlFieldName) && clientValidation == null)
            return validation;
            ModelState modelState = html.ViewData.ModelState[fullHtmlFieldName];
            if (modelState != null)
                validation = "is-valid";
            ModelErrorCollection source = modelState == null ? (ModelErrorCollection)null : modelState.Errors;
            ModelError error = source == null || source.Count == 0 ? (ModelError)null : source.FirstOrDefault<ModelError>((Func<ModelError, bool>)(m => !string.IsNullOrEmpty(m.ErrorMessage))) ?? source[0];
            if (error == null && clientValidation == null)
                return validation;
            
            if (error != null)
                validation = "is-invalid";
            return validation;
        }
        
        internal static FormContext GetFormContextForClientValidation(this ViewContext viewContext)
        {
            if (!viewContext.ClientValidationEnabled)
                return (FormContext)null;
            return viewContext.FormContext;
        }
        #endregion
        #region ValidationDropDownListFor
        /// <summary>Возвращает HTML-элемент select для каждого свойства объекта, представленного указанным выражением, используя указанные элементы списка.</summary>
        /// <returns>HTML-элемент select для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="htmlHelper">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="selectList">Коллекция объектов <see cref="T:System.Web.Mvc.SelectListItem" />, которые используются для заполнения раскрывающегося списка.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TProperty">Тип значения.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="expression" /> равен null.</exception>
        public static MvcHtmlString ValidationDropDownListFor<TModel, TProperty>(
          this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TProperty>> expression,
          IEnumerable<SelectListItem> selectList)
        {
            return ValidationDropDownListForHelper<TModel, TProperty>(htmlHelper, expression, selectList, (string)null, (IDictionary<string, object>)null);
        }

        /// <summary>Возвращает HTML-элемент select для каждого свойства объекта, представленного указанным выражением, используя указанные элементы списка и атрибуты HTML.</summary>
        /// <returns>HTML-элемент select для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="htmlHelper">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="selectList">Коллекция объектов <see cref="T:System.Web.Mvc.SelectListItem" />, которые используются для заполнения раскрывающегося списка.</param>
        /// <param name="htmlAttributes">Объект, содержащий атрибуты HTML, которые следует задать для элемента.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TProperty">Тип значения.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="expression" /> равен null.</exception>
        public static MvcHtmlString ValidationDropDownListFor<TModel, TProperty>(
          this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TProperty>> expression,
          IEnumerable<SelectListItem> selectList,
          object htmlAttributes)
        {
            return ValidationDropDownListForHelper<TModel, TProperty>(htmlHelper, expression, selectList, (string)null, (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>Возвращает HTML-элемент select для каждого свойства объекта, представленного указанным выражением, используя указанные элементы списка и атрибуты HTML.</summary>
        /// <returns>HTML-элемент select для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="htmlHelper">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="selectList">Коллекция объектов <see cref="T:System.Web.Mvc.SelectListItem" />, которые используются для заполнения раскрывающегося списка.</param>
        /// <param name="htmlAttributes">Объект, содержащий атрибуты HTML, которые следует задать для элемента.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TProperty">Тип значения.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="expression" /> равен null.</exception>
        public static MvcHtmlString ValidationDropDownListFor<TModel, TProperty>(
          this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TProperty>> expression,
          IEnumerable<SelectListItem> selectList,
          IDictionary<string, object> htmlAttributes)
        {
            return ValidationDropDownListForHelper<TModel, TProperty>(htmlHelper, expression, selectList, (string)null, htmlAttributes);
        }

        /// <summary>Возвращает HTML-элемент select для каждого свойства объекта, представленного указанным выражением, используя указанные элементы списка и метку варианта.</summary>
        /// <returns>HTML-элемент select для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="htmlHelper">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="selectList">Коллекция объектов <see cref="T:System.Web.Mvc.SelectListItem" />, которые используются для заполнения раскрывающегося списка.</param>
        /// <param name="optionLabel">Текст для пустого элемента по умолчанию.Этот параметр может иметь значение null.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TProperty">Тип значения.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="expression" /> равен null.</exception>
        public static MvcHtmlString ValidationDropDownListFor<TModel, TProperty>(
          this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TProperty>> expression,
          IEnumerable<SelectListItem> selectList,
          string optionLabel)
        {
            return ValidationDropDownListForHelper<TModel, TProperty>(htmlHelper, expression, selectList, optionLabel, (IDictionary<string, object>)null);
        }

        /// <summary>Возвращает HTML-элемент select для каждого свойства объекта, представленного указанным выражением, используя указанные элементы списка, метку варианта и атрибуты HTML.</summary>
        /// <returns>HTML-элемент select для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="htmlHelper">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="selectList">Коллекция объектов <see cref="T:System.Web.Mvc.SelectListItem" />, которые используются для заполнения раскрывающегося списка.</param>
        /// <param name="optionLabel">Текст для пустого элемента по умолчанию.Этот параметр может иметь значение null.</param>
        /// <param name="htmlAttributes">Объект, содержащий атрибуты HTML, которые следует задать для элемента.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TProperty">Тип значения.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="expression" /> равен null.</exception>
        public static MvcHtmlString ValidationDropDownListFor<TModel, TProperty>(
          this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TProperty>> expression,
          IEnumerable<SelectListItem> selectList,
          string optionLabel,
          object htmlAttributes)
        {
            return ValidationDropDownListForHelper<TModel, TProperty>(htmlHelper, expression, selectList, optionLabel, (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>Возвращает HTML-элемент select для каждого свойства объекта, представленного указанным выражением, используя указанные элементы списка, метку варианта и атрибуты HTML.</summary>
        /// <returns>HTML-элемент select для каждого свойства объекта, представленного выражением.</returns>
        /// <param name="htmlHelper">Экземпляр вспомогательного метода HTML, который расширяется данным методом.</param>
        /// <param name="expression">Выражение, которое определяет объект, содержащий свойства для отображения.</param>
        /// <param name="selectList">Коллекция объектов <see cref="T:System.Web.Mvc.SelectListItem" />, которые используются для заполнения раскрывающегося списка.</param>
        /// <param name="optionLabel">Текст для пустого элемента по умолчанию.Этот параметр может иметь значение null.</param>
        /// <param name="htmlAttributes">Объект, содержащий атрибуты HTML, которые следует задать для элемента.</param>
        /// <typeparam name="TModel">Тип модели.</typeparam>
        /// <typeparam name="TProperty">Тип значения.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="expression" /> равен null.</exception>
        public static MvcHtmlString ValidationDropDownListForHelper<TModel, TProperty>(
          this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TProperty>> expression,
          IEnumerable<SelectListItem> selectList,
          string optionLabel,
          IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            string validation = htmlHelper.ValidationCheck(ExpressionHelper.GetExpressionText((LambdaExpression)expression));
            if (validation != "")
            {
                if (htmlAttributes.ContainsKey("class"))
                {
                    htmlAttributes["class"] = htmlAttributes["class"] + " " + validation;
                }
                else
                {
                    htmlAttributes.Add("class", validation);
                }
            }
            return SelectExtensions.DropDownListFor<TModel, TProperty>(htmlHelper, expression, selectList, optionLabel, htmlAttributes);
        }
        #endregion
        public static MvcHtmlString DataList<TModel>(this HtmlHelper<TModel> htmlHelper, List<string> list, string id)
        {
            StringBuilder stringBuilder = new StringBuilder();
            TagBuilder tagBuilder = new TagBuilder("datalist");
            tagBuilder.MergeAttribute("id", id);
            stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.StartTag));

            foreach (string text in list)
            {
                tagBuilder = new TagBuilder("option");
                tagBuilder.SetInnerText(text);
                stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.Normal));
            }
            tagBuilder = new TagBuilder("datalist");
            stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.EndTag));
            return MvcHtmlString.Create(stringBuilder.ToString());

        }
        public static MvcHtmlString ManyToManyCheckBoxList<T>(
          this HtmlHelper htmlHelper,
          string CheckBoxesName,
          IEnumerable<T> modelList,
          IEnumerable<T> selectList,
          string rowCssClass, string colCssClass)
        {
            List<T> list = selectList.ToList();
            modelList = modelList == null ? new List<T>() : modelList; 
            int add = list.Count % 3;
            int count = list.Count / 3;
            StringBuilder stringBuilder = new StringBuilder();
            
            for (int i = 0; i < (count + add); i++)
            {
                TagBuilder tagBuilder = new TagBuilder("div");
                tagBuilder.MergeAttribute("class", "row" + (String.IsNullOrEmpty(rowCssClass) ? "" : " " + rowCssClass));
                stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.StartTag));
                tagBuilder = new TagBuilder("div");
                tagBuilder.MergeAttribute("class", "col" + (String.IsNullOrEmpty(colCssClass) ? "" : " " + colCssClass));
                stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.StartTag));
                tagBuilder = new TagBuilder("input");
                tagBuilder.MergeAttribute("type", "checkbox");
                tagBuilder.MergeAttribute("name", CheckBoxesName);
                
                if (modelList.Any(x => x.EqualId(list[i])))
                {
                    tagBuilder.MergeAttribute("checked", "checked");
                }
                var obj1Type = list[i].GetType();
                
                tagBuilder.MergeAttribute("value", obj1Type.GetProperty("Id").GetValue(list[i], null).ToString());
                stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.SelfClosing));
                stringBuilder.AppendLine(" " + obj1Type.GetProperty("Name").GetValue(list[i], null).ToString());
                tagBuilder = new TagBuilder("div");
                stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.EndTag));
                if ((i + count + add) >= (count + add) & (i + count + add) < (2 * count + add))
                {
                    tagBuilder = new TagBuilder("div");
                    tagBuilder.MergeAttribute("class", "col" + (String.IsNullOrEmpty(colCssClass) ? "" : " " + colCssClass));
                    stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.StartTag));
                    tagBuilder = new TagBuilder("input");
                    tagBuilder.MergeAttribute("type", "checkbox");
                    tagBuilder.MergeAttribute("name", CheckBoxesName);
                    if (modelList.Any(x => x.EqualId(list[i + count + add])))
                    {
                        tagBuilder.MergeAttribute("checked", "checked");
                    }
                    tagBuilder.MergeAttribute("value", obj1Type.GetProperty("Id").GetValue(list[i + count + add], null).ToString());
                    stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.SelfClosing));
                    stringBuilder.AppendLine(" " + obj1Type.GetProperty("Name").GetValue(list[i + count + add], null).ToString());
                    tagBuilder = new TagBuilder("div");
                    stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.EndTag));
                }
                else
                {
                    tagBuilder = new TagBuilder("div");
                    tagBuilder.MergeAttribute("class", "col" + (String.IsNullOrEmpty(colCssClass) ? "" : " " + colCssClass));
                    stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.Normal));
                }
                if ((i + (2 * count) + add) < list.Count)
                {
                    tagBuilder = new TagBuilder("div");
                    tagBuilder.MergeAttribute("class", "col" + (String.IsNullOrEmpty(colCssClass) ? "" : " " + colCssClass));
                    stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.StartTag));
                    tagBuilder = new TagBuilder("input");
                    tagBuilder.MergeAttribute("type", "checkbox");
                    tagBuilder.MergeAttribute("name", CheckBoxesName);
                    if (modelList.Any(x => x.EqualId(list[i + (2 * count) + add])))
                    {
                        tagBuilder.MergeAttribute("checked", "checked");
                    }
                    tagBuilder.MergeAttribute("value", obj1Type.GetProperty("Id").GetValue(list[i + (2 * count) + add], null).ToString());
                    stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.SelfClosing));
                    stringBuilder.AppendLine(" " + obj1Type.GetProperty("Name").GetValue(list[i + (2 * count) + add], null).ToString());
                    tagBuilder = new TagBuilder("div");
                    stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.EndTag));
                    
                }
                else
                {
                    tagBuilder = new TagBuilder("div");
                    tagBuilder.MergeAttribute("class", "col" + (String.IsNullOrEmpty(colCssClass) ? "" : " " + colCssClass));
                    stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.Normal));
                }
                tagBuilder = new TagBuilder("div");
                stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.EndTag));
            }

            return MvcHtmlString.Create(stringBuilder.ToString());
        }
    }
    public static class LangHelper
    {
        public static MvcHtmlString LangSwitcher(this UrlHelper url, string Name, RouteData routeData, string lang)
        {
            
            var aTagBuilder = new TagBuilder("a");
            var routeValueDictionary = new RouteValueDictionary(routeData.Values);
            aTagBuilder.Attributes["class"] = "dropdown-item";
            if (routeValueDictionary.ContainsKey("lang"))
            {
                if (routeData.Values["lang"] as string == lang)
                {
                    aTagBuilder.AddCssClass("active");
                }
                else
                {
                    routeValueDictionary["lang"] = lang;
                }
            }
            aTagBuilder.MergeAttribute("href", url.RouteUrl(routeValueDictionary));
            aTagBuilder.SetInnerText(Name);
            return new MvcHtmlString(aTagBuilder.ToString());
        }
    }

    /// <summary>Represents support for ASP.NET AJAX within an ASP.NET MVC application.</summary>
    public static class CustomAjaxExtensions
    {
        private const string LinkOnClickFormat = "Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), {0});";
        private const string FormOnClickValue = "Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));";
        private const string FormOnSubmitFormat = "Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), {0});";

        /// <summary>Returns an anchor element that contains the URL to the specified action method; when the action link is clicked, the action method is invoked asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomActionLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string actionName,
          AjaxOptions ajaxOptions)
        {
            return CustomActionLink(ajaxHelper, linkText, actionName, (string)null, ajaxOptions);
        }

        /// <summary>Returns an anchor element that contains the URL to the specified action method; when the action link is clicked, the action method is invoked asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomActionLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string actionName,
          object routeValues,
          AjaxOptions ajaxOptions)
        {
            return ajaxHelper.CustomActionLink(linkText, actionName, (string)null, routeValues, ajaxOptions);
        }

        /// <summary>Returns an anchor element that contains the URL to the specified action method; when the action link is clicked, the action method is invoked asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomActionLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string actionName,
          object routeValues,
          AjaxOptions ajaxOptions,
          object htmlAttributes)
        {
            return ajaxHelper.CustomActionLink(linkText, actionName, (string)null, routeValues, ajaxOptions, htmlAttributes,null);
        }

        /// <summary>Returns an anchor element that contains the URL to the specified action method; when the action link is clicked, the action method is invoked asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomActionLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string actionName,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions)
        {
            return CustomActionLink(ajaxHelper, linkText, actionName, (string)null, routeValues, ajaxOptions);
        }

        /// <summary>Returns an anchor element that contains the URL to the specified action method; when the action link is clicked, the action method is invoked asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomActionLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string actionName,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions,
          IDictionary<string, object> htmlAttributes)
        {
            return CustomActionLink(ajaxHelper, linkText, actionName, (string)null, routeValues, ajaxOptions, htmlAttributes,null);
        }

        /// <summary>Returns an anchor element that contains the URL to the specified action method; when the action link is clicked, the action method is invoked asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomActionLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string actionName,
          string controllerName,
          AjaxOptions ajaxOptions)
        {
            return CustomActionLink(ajaxHelper, linkText, actionName, controllerName, (RouteValueDictionary)null, ajaxOptions, (IDictionary<string, object>)null,null);
        }

        /// <summary>Returns an anchor element that contains the URL to the specified action method; when the action link is clicked, the action method is invoked asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomActionLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string actionName,
          string controllerName,
          object routeValues,
          AjaxOptions ajaxOptions)
        {
            return ajaxHelper.CustomActionLink(linkText, actionName, controllerName, routeValues, ajaxOptions, (object)null,null);
        }

        /// <summary>Returns an anchor element that contains the URL to the specified action method; when the action link is clicked, the action method is invoked asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomActionLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string actionName,
          string controllerName,
          object routeValues,
          AjaxOptions ajaxOptions,
          object htmlAttributes,
          string iconClass)
        {
            RouteValueDictionary dictionary = TypeHelper.ObjectToDictionary(routeValues);
            RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return CustomActionLink(ajaxHelper, linkText, actionName, controllerName, dictionary, ajaxOptions, (IDictionary<string, object>)htmlAttributes1, iconClass);
        }

        /// <summary>Returns an anchor element that contains the URL to the specified action method; when the action link is clicked, the action method is invoked asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomActionLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string actionName,
          string controllerName,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions)
        {
            return CustomActionLink(ajaxHelper, linkText, actionName, controllerName, routeValues, ajaxOptions, (IDictionary<string, object>)null,null);
        }

        /// <summary>Returns an anchor element that contains the URL to the specified action method; when the action link is clicked, the action method is invoked asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomActionLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string actionName,
          string controllerName,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions,
          IDictionary<string, object> htmlAttributes,
          string iconClass)
        {
            if (string.IsNullOrEmpty(linkText))
                throw new ArgumentException(GlobalRes.NullOrEmpty, nameof(linkText));
            string url = UrlHelper.GenerateUrl((string)null, actionName, controllerName, routeValues, ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, true);
            return MvcHtmlString.Create(GenerateLink(ajaxHelper, linkText, url, GetAjaxOptions(ajaxOptions), htmlAttributes, iconClass));
        }

        /// <summary>Returns an anchor element that contains the URL to the specified action method; when the action link is clicked, the action method is invoked asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="hostName">The host name for the URL.</param>
        /// <param name="fragment">The URL fragment name (the anchor name).</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomActionLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string actionName,
          string controllerName,
          string protocol,
          string hostName,
          string fragment,
          object routeValues,
          AjaxOptions ajaxOptions,
          object htmlAttributes)
        {
            RouteValueDictionary dictionary = TypeHelper.ObjectToDictionary(routeValues);
            RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return CustomActionLink(ajaxHelper, linkText, actionName, controllerName, protocol, hostName, fragment, dictionary, ajaxOptions, (IDictionary<string, object>)htmlAttributes1);
        }

        /// <summary>Returns an anchor element that contains the URL to the specified action method; when the action link is clicked, the action method is invoked asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="hostName">The host name for the URL.</param>
        /// <param name="fragment">The URL fragment name (the anchor name).</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomActionLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string actionName,
          string controllerName,
          string protocol,
          string hostName,
          string fragment,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions,
          IDictionary<string, object> htmlAttributes,
          string iconClass)
        {
            if (string.IsNullOrEmpty(linkText))
                throw new ArgumentException(GlobalRes.NullOrEmpty, nameof(linkText));
            string url = UrlHelper.GenerateUrl((string)null, actionName, controllerName, protocol, hostName, fragment, routeValues, ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, true);
            return MvcHtmlString.Create(GenerateLink(ajaxHelper, linkText, url, ajaxOptions, htmlAttributes, iconClass));
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        public static MvcForm CustomBeginForm(this AjaxHelper ajaxHelper, AjaxOptions ajaxOptions)
        {
            string rawUrl = ajaxHelper.ViewContext.HttpContext.Request.RawUrl;
            return ajaxHelper.CustomFormHelper(rawUrl, ajaxOptions, (IDictionary<string, object>)new RouteValueDictionary());
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        public static MvcForm CustomBeginForm(
          this AjaxHelper ajaxHelper,
          string actionName,
          AjaxOptions ajaxOptions)
        {
            return CustomBeginForm(ajaxHelper, actionName, (string)null, ajaxOptions);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        public static MvcForm CustomBeginForm(
          this AjaxHelper ajaxHelper,
          string actionName,
          object routeValues,
          AjaxOptions ajaxOptions)
        {
            return ajaxHelper.CustomBeginForm(actionName, (string)null, routeValues, ajaxOptions);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm CustomBeginForm(
          this AjaxHelper ajaxHelper,
          string actionName,
          object routeValues,
          AjaxOptions ajaxOptions,
          object htmlAttributes)
        {
            return ajaxHelper.CustomBeginForm(actionName, (string)null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        public static MvcForm CustomBeginForm(
          this AjaxHelper ajaxHelper,
          string actionName,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions)
        {
            return CustomBeginForm(ajaxHelper, actionName, (string)null, routeValues, ajaxOptions);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response. </summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element..</param>
        public static MvcForm CustomBeginForm(
          this AjaxHelper ajaxHelper,
          string actionName,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions,
          IDictionary<string, object> htmlAttributes)
        {
            return CustomBeginForm(ajaxHelper, actionName, (string)null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        public static MvcForm CustomBeginForm(
          this AjaxHelper ajaxHelper,
          string actionName,
          string controllerName,
          AjaxOptions ajaxOptions)
        {
            return CustomBeginForm(ajaxHelper, actionName, controllerName, (RouteValueDictionary)null, ajaxOptions, (IDictionary<string, object>)null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        public static MvcForm CustomBeginForm(
          this AjaxHelper ajaxHelper,
          string actionName,
          string controllerName,
          object routeValues,
          AjaxOptions ajaxOptions)
        {
            return ajaxHelper.CustomBeginForm(actionName, controllerName, routeValues, ajaxOptions, (object)null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm CustomBeginForm(
          this AjaxHelper ajaxHelper,
          string actionName,
          string controllerName,
          object routeValues,
          AjaxOptions ajaxOptions,
          object htmlAttributes)
        {
            RouteValueDictionary routeValues1 = new RouteValueDictionary(routeValues);
            RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return CustomBeginForm(ajaxHelper, actionName, controllerName, routeValues1, ajaxOptions, (IDictionary<string, object>)htmlAttributes1);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        public static MvcForm CustomBeginForm(
          this AjaxHelper ajaxHelper,
          string actionName,
          string controllerName,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions)
        {
            return CustomBeginForm(ajaxHelper, actionName, controllerName, routeValues, ajaxOptions, (IDictionary<string, object>)null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm CustomBeginForm(
          this AjaxHelper ajaxHelper,
          string actionName,
          string controllerName,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions,
          IDictionary<string, object> htmlAttributes)
        {
            string url = UrlHelper.GenerateUrl((string)null, actionName, controllerName, routeValues ?? new RouteValueDictionary(), ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, true);
            return ajaxHelper.CustomFormHelper(url, ajaxOptions, htmlAttributes);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response using the specified routing information.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        public static MvcForm CustomBeginRouteForm(
          this AjaxHelper ajaxHelper,
          string routeName,
          AjaxOptions ajaxOptions)
        {
            return CustomBeginRouteForm(ajaxHelper, routeName, (RouteValueDictionary)null, ajaxOptions, (IDictionary<string, object>)null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response using the specified routing information.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        public static MvcForm CustomBeginRouteForm(
          this AjaxHelper ajaxHelper,
          string routeName,
          object routeValues,
          AjaxOptions ajaxOptions)
        {
            return ajaxHelper.CustomBeginRouteForm(routeName, routeValues, ajaxOptions, (object)null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response using the specified routing information.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm CustomBeginRouteForm(
          this AjaxHelper ajaxHelper,
          string routeName,
          object routeValues,
          AjaxOptions ajaxOptions,
          object htmlAttributes)
        {
            RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return CustomBeginRouteForm(ajaxHelper, routeName, new RouteValueDictionary(routeValues), ajaxOptions, (IDictionary<string, object>)htmlAttributes1);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response using the specified routing information.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        public static MvcForm CustomBeginRouteForm(
          this AjaxHelper ajaxHelper,
          string routeName,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions)
        {
            return CustomBeginRouteForm(ajaxHelper, routeName, routeValues, ajaxOptions, (IDictionary<string, object>)null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response using the specified routing information.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm CustomBeginRouteForm(
          this AjaxHelper ajaxHelper,
          string routeName,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions,
          IDictionary<string, object> htmlAttributes)
        {
            string url = UrlHelper.GenerateUrl(routeName, (string)null, (string)null, routeValues ?? new RouteValueDictionary(), ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, false);
            return ajaxHelper.CustomFormHelper(url, ajaxOptions, htmlAttributes);
        }

        private static MvcForm CustomFormHelper(
          this AjaxHelper ajaxHelper,
          string formAction,
          AjaxOptions ajaxOptions,
          IDictionary<string, object> htmlAttributes)
        {
            TagBuilder tagBuilder = new TagBuilder("form");
            tagBuilder.MergeAttributes<string, object>(htmlAttributes);
            tagBuilder.MergeAttribute("action", formAction);
            tagBuilder.MergeAttribute("method", "post");
            ajaxOptions = GetAjaxOptions(ajaxOptions);
            if (ajaxHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
            {
                tagBuilder.MergeAttributes<string, object>(ajaxOptions.ToUnobtrusiveHtmlAttributes());
            }
            else
            {
                tagBuilder.MergeAttribute("onclick", "Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));");
                tagBuilder.MergeAttribute("onsubmit", GenerateAjaxScript(ajaxOptions, "Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), {0});"));
            }
            if (ajaxHelper.ViewContext.ClientValidationEnabled)
                tagBuilder.GenerateId(ajaxHelper.ViewContext.FormIdGenerator());
            ajaxHelper.ViewContext.Writer.Write(tagBuilder.ToString((TagRenderMode)1));
            MvcForm mvcForm = new MvcForm(ajaxHelper.ViewContext);
            if (!ajaxHelper.ViewContext.ClientValidationEnabled)
                return mvcForm;
            ajaxHelper.ViewContext.FormContext.FormId = tagBuilder.Attributes["id"];
            return mvcForm;
        }



        /// <summary>Returns an HTML script element that contains a reference to a globalization script that defines the culture information.</summary>
        /// <returns>A script element whose src attribute is set to the globalization script, as in the following example: &lt;script type="text/javascript"     src="/MvcApplication1/Scripts/Globalization/en-US.js"&gt;&lt;/script&gt;</returns>
        /// <param name="ajaxHelper">The AJAX helper object that this method extends.</param>
        public static MvcHtmlString CustomGlobalizationScript(this AjaxHelper ajaxHelper)
        {
            return ajaxHelper.CustomGlobalizationScript(CultureInfo.CurrentCulture);
        }

        /// <summary>Returns an HTML script element that contains a reference to a globalization script that defines the specified culture information.</summary>
        /// <returns>An HTML script element whose src attribute is set to the globalization script, as in the following example:&lt;script type="text/javascript"    src="/MvcApplication1/Scripts/Globalization/en-US.js"&gt;&lt;/script&gt;</returns>
        /// <param name="ajaxHelper">The AJAX helper object that this method extends.</param>
        /// <param name="cultureInfo">Encapsulates information about the target culture, such as date formats.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="cultureInfo" /> parameter is null.</exception>
        public static MvcHtmlString CustomGlobalizationScript(
          this AjaxHelper ajaxHelper,
          CultureInfo cultureInfo)
        {
            return GlobalizationScriptHelper(AjaxHelper.GlobalizationScriptPath, cultureInfo);
        }

        internal static MvcHtmlString GlobalizationScriptHelper(
          string scriptPath,
          CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
                throw new ArgumentNullException(nameof(cultureInfo));
            TagBuilder tagBuilder = new TagBuilder("script");
            tagBuilder.MergeAttribute("type", "text/javascript");
            tagBuilder.MergeAttribute("src", VirtualPathUtility.AppendTrailingSlash(scriptPath) + HttpUtility.UrlEncode(cultureInfo.Name) + ".js");
            return tagBuilder.ToMvcHtmlString((TagRenderMode)0);
        }

        /// <summary>Returns an anchor element that contains the virtual path for the specified route values; when the link is clicked, a request is made to the virtual path asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomRouteLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          object routeValues,
          AjaxOptions ajaxOptions)
        {
            return CustomRouteLink(ajaxHelper, linkText, (string)null, new RouteValueDictionary(routeValues), ajaxOptions, (IDictionary<string, object>)new Dictionary<string, object>());
        }

        /// <summary>Returns an anchor element that contains the virtual path for the specified route values; when the link is clicked, a request is made to the virtual path asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomRouteLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          object routeValues,
          AjaxOptions ajaxOptions,
          object htmlAttributes)
        {
            return CustomRouteLink(ajaxHelper, linkText, (string)null, new RouteValueDictionary(routeValues), ajaxOptions, (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>Returns an anchor element that contains the virtual path for the specified route values; when the link is clicked, a request is made to the virtual path asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomRouteLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions)
        {
            return CustomRouteLink(ajaxHelper, linkText, (string)null, routeValues, ajaxOptions, (IDictionary<string, object>)new Dictionary<string, object>());
        }

        /// <summary>Returns an anchor element that contains the virtual path for the specified route values; when the link is clicked, a request is made to the virtual path asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomRouteLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions,
          IDictionary<string, object> htmlAttributes)
        {
            return CustomRouteLink(ajaxHelper, linkText, (string)null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>Returns an anchor element that contains the virtual path for the specified route values; when the link is clicked, a request is made to the virtual path asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomRouteLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string routeName,
          AjaxOptions ajaxOptions)
        {
            return CustomRouteLink(ajaxHelper, linkText, routeName, new RouteValueDictionary(), ajaxOptions, (IDictionary<string, object>)new Dictionary<string, object>());
        }

        /// <summary>Returns an anchor element that contains the virtual path for the specified route values; when the link is clicked, a request is made to the virtual path asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomRouteLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string routeName,
          AjaxOptions ajaxOptions,
          object htmlAttributes)
        {
            return CustomRouteLink(ajaxHelper, linkText, routeName, new RouteValueDictionary(), ajaxOptions, (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>Returns an anchor element that contains the virtual path for the specified route values; when the link is clicked, a request is made to the virtual path asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomRouteLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string routeName,
          AjaxOptions ajaxOptions,
          IDictionary<string, object> htmlAttributes)
        {
            return CustomRouteLink(ajaxHelper, linkText, routeName, new RouteValueDictionary(), ajaxOptions, htmlAttributes);
        }

        /// <summary>Returns an anchor element that contains the virtual path for the specified route values; when the link is clicked, a request is made to the virtual path asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomRouteLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string routeName,
          object routeValues,
          AjaxOptions ajaxOptions)
        {
            return CustomRouteLink(ajaxHelper, linkText, routeName, new RouteValueDictionary(routeValues), ajaxOptions, (IDictionary<string, object>)new Dictionary<string, object>());
        }

        /// <summary>Returns an anchor element that contains the virtual path for the specified route values; when the link is clicked, a request is made to the virtual path asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomRouteLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string routeName,
          object routeValues,
          AjaxOptions ajaxOptions,
          object htmlAttributes)
        {
            return CustomRouteLink(ajaxHelper, linkText, routeName, new RouteValueDictionary(routeValues), ajaxOptions, (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>Returns an anchor element that contains the virtual path for the specified route values; when the link is clicked, a request is made to the virtual path asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomRouteLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string routeName,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions)
        {
            return CustomRouteLink(ajaxHelper, linkText, routeName, routeValues, ajaxOptions, (IDictionary<string, object>)new Dictionary<string, object>());
        }

        /// <summary>Returns an anchor element that contains the virtual path for the specified route values; when the link is clicked, a request is made to the virtual path asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomRouteLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string routeName,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions,
          IDictionary<string, object> htmlAttributes)
        {
            if (string.IsNullOrEmpty(linkText))
                throw new ArgumentException(GlobalRes.NullOrEmpty, nameof(linkText));
            string url = UrlHelper.GenerateUrl(routeName, (string)null, (string)null, routeValues ?? new RouteValueDictionary(), ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, false);
            return MvcHtmlString.Create(GenerateLink(ajaxHelper, linkText, url, GetAjaxOptions(ajaxOptions), htmlAttributes,null));
        }

        /// <summary>Returns an anchor element that contains the virtual path for the specified route values; when the link is clicked, a request is made to the virtual path asynchronously by using JavaScript.</summary>
        /// <returns>An anchor element.</returns>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="routeName">The name of the route to use to obtain the form post URL.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="hostName">The host name for the URL.</param>
        /// <param name="fragment">The URL fragment name (the anchor name).</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
        public static MvcHtmlString CustomRouteLink(
          this AjaxHelper ajaxHelper,
          string linkText,
          string routeName,
          string protocol,
          string hostName,
          string fragment,
          RouteValueDictionary routeValues,
          AjaxOptions ajaxOptions,
          IDictionary<string, object> htmlAttributes)
        {
            if (string.IsNullOrEmpty(linkText))
                throw new ArgumentException(GlobalRes.NullOrEmpty, nameof(linkText));
            string url = UrlHelper.GenerateUrl(routeName, (string)null, (string)null, protocol, hostName, fragment, routeValues ?? new RouteValueDictionary(), ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, false);
            return MvcHtmlString.Create(GenerateLink(ajaxHelper, linkText, url, GetAjaxOptions(ajaxOptions), htmlAttributes,null));
        }

        private static string GenerateLink(
          AjaxHelper ajaxHelper,
          string linkText,
          string targetUrl,
          AjaxOptions ajaxOptions,
          IDictionary<string, object> htmlAttributes,
          string icon)
        {
            StringBuilder stringBuilder = new StringBuilder();
            TagBuilder tagBuilder1 = new TagBuilder("a");
            if (!String.IsNullOrEmpty(icon))
            {
               if (String.IsNullOrEmpty(ajaxOptions.LoadingElementId))
                {
                    Random random = new Random();
                    ajaxOptions.LoadingElementId = "load" + random.Next(0, 100000);
                }
            }
            //tagBuilder1.InnerHtml = HttpUtility.HtmlEncode(linkText);
            TagBuilder tagBuilder2 = tagBuilder1;
            tagBuilder2.MergeAttributes<string, object>(htmlAttributes);
            tagBuilder2.MergeAttribute("href", targetUrl);
            if (ajaxHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                tagBuilder2.MergeAttributes<string, object>(ajaxOptions.ToUnobtrusiveHtmlAttributes());
            else
                tagBuilder2.MergeAttribute("onclick", GenerateAjaxScript(ajaxOptions, "Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), {0});"));
            stringBuilder.AppendLine(tagBuilder2.ToString(TagRenderMode.StartTag));
            if (!String.IsNullOrEmpty(icon))
            {
                TagBuilder iconTag = new TagBuilder("i");
                iconTag.AddCssClass(icon);
                iconTag.GenerateId(ajaxOptions.LoadingElementId);
                iconTag.Attributes.Add("style", "display:none");
                stringBuilder.AppendLine(iconTag.ToString(TagRenderMode.Normal));
                linkText = " " + linkText;
            }
            stringBuilder.AppendLine(HttpUtility.HtmlEncode(linkText));
            stringBuilder.AppendLine(tagBuilder2.ToString(TagRenderMode.EndTag));
            return stringBuilder.ToString();
        }

        private static string GenerateAjaxScript(AjaxOptions ajaxOptions, string scriptFormat)
        {
            string javascriptString = ajaxOptions.ToJavascriptString();
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, scriptFormat, (object)javascriptString);
        }

        private static AjaxOptions GetAjaxOptions(AjaxOptions ajaxOptions)
        {
            if (ajaxOptions == null)
                return new AjaxOptions();
            return ajaxOptions;
        }

        internal class PropertyHelper
        {
            private static ConcurrentDictionary<Type, PropertyHelper[]> _reflectionCache = new ConcurrentDictionary<Type, PropertyHelper[]>();
            private static readonly MethodInfo _callPropertyGetterOpenGenericMethod = typeof(PropertyHelper).GetMethod("CallPropertyGetter", BindingFlags.Static | BindingFlags.NonPublic);
            private static readonly MethodInfo _callPropertyGetterByReferenceOpenGenericMethod = typeof(PropertyHelper).GetMethod("CallPropertyGetterByReference", BindingFlags.Static | BindingFlags.NonPublic);
            private static readonly MethodInfo _callPropertySetterOpenGenericMethod = typeof(PropertyHelper).GetMethod("CallPropertySetter", BindingFlags.Static | BindingFlags.NonPublic);
            private Func<object, object> _valueGetter;

            public PropertyHelper(PropertyInfo property)
            {
                this.Name = property.Name;
                this._valueGetter = PropertyHelper.MakeFastPropertyGetter(property);
            }

            public static Action<TDeclaringType, object> MakeFastPropertySetter<TDeclaringType>(
              PropertyInfo propertyInfo)
              where TDeclaringType : class
            {
                MethodInfo setMethod = propertyInfo.GetSetMethod();
                Type reflectedType = propertyInfo.ReflectedType;
                Type parameterType = setMethod.GetParameters()[0].ParameterType;
                return (Action<TDeclaringType, object>)Delegate.CreateDelegate(typeof(Action<TDeclaringType, object>), (object)setMethod.CreateDelegate(typeof(Action<,>).MakeGenericType(reflectedType, parameterType)), PropertyHelper._callPropertySetterOpenGenericMethod.MakeGenericMethod(reflectedType, parameterType));
            }

            public virtual string Name { get; protected set; }

            public object GetValue(object instance)
            {
                return this._valueGetter(instance);
            }

            public static PropertyHelper[] GetProperties(object instance)
            {
                return PropertyHelper.GetProperties(instance, new Func<PropertyInfo, PropertyHelper>(PropertyHelper.CreateInstance), PropertyHelper._reflectionCache);
            }

            public static Func<object, object> MakeFastPropertyGetter(PropertyInfo propertyInfo)
            {
                MethodInfo getMethod = propertyInfo.GetGetMethod();
                Type reflectedType = getMethod.ReflectedType;
                Type returnType = getMethod.ReturnType;
                Delegate @delegate;
                if (reflectedType.IsValueType)
                    @delegate = Delegate.CreateDelegate(typeof(Func<object, object>), (object)getMethod.CreateDelegate(typeof(PropertyHelper.ByRefFunc<,>).MakeGenericType(reflectedType, returnType)), PropertyHelper._callPropertyGetterByReferenceOpenGenericMethod.MakeGenericMethod(reflectedType, returnType));
                else
                    @delegate = Delegate.CreateDelegate(typeof(Func<object, object>), (object)getMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(reflectedType, returnType)), PropertyHelper._callPropertyGetterOpenGenericMethod.MakeGenericMethod(reflectedType, returnType));
                return (Func<object, object>)@delegate;
            }

            private static PropertyHelper CreateInstance(PropertyInfo property)
            {
                return new PropertyHelper(property);
            }

            private static object CallPropertyGetter<TDeclaringType, TValue>(
              Func<TDeclaringType, TValue> getter,
              object @this)
            {
                return (object)getter((TDeclaringType)@this);
            }

            private static object CallPropertyGetterByReference<TDeclaringType, TValue>(
              PropertyHelper.ByRefFunc<TDeclaringType, TValue> getter,
              object @this)
            {
                TDeclaringType declaringType = (TDeclaringType)@this;
                return (object)getter(ref declaringType);
            }

            private static void CallPropertySetter<TDeclaringType, TValue>(
              Action<TDeclaringType, TValue> setter,
              object @this,
              object value)
            {
                setter((TDeclaringType)@this, (TValue)value);
            }

            protected static PropertyHelper[] GetProperties(
              object instance,
              Func<PropertyInfo, PropertyHelper> createPropertyHelper,
              ConcurrentDictionary<Type, PropertyHelper[]> cache)
            {
                Type type = instance.GetType();
                PropertyHelper[] array;
                if (!cache.TryGetValue(type, out array))
                {
                    IEnumerable<PropertyInfo> propertyInfos = ((IEnumerable<PropertyInfo>)type.GetProperties(BindingFlags.Instance | BindingFlags.Public)).Where<PropertyInfo>((Func<PropertyInfo, bool>)(prop =>
                    {
                        if (prop.GetIndexParameters().Length == 0)
                            return prop.GetMethod != (MethodInfo)null;
                        return false;
                    }));
                    List<PropertyHelper> propertyHelperList = new List<PropertyHelper>();
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        PropertyHelper propertyHelper = createPropertyHelper(propertyInfo);
                        propertyHelperList.Add(propertyHelper);
                    }
                    array = propertyHelperList.ToArray();
                    cache.TryAdd(type, array);
                }
                return array;
            }

            private delegate TValue ByRefFunc<TDeclaringType, TValue>(ref TDeclaringType arg);
        }
        internal static class TypeHelper
        {
            public static RouteValueDictionary ObjectToDictionary(object value)
            {
                RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
                if (value != null)
                {
                    foreach (PropertyHelper property in PropertyHelper.GetProperties(value))
                        routeValueDictionary.Add(property.Name, property.GetValue(value));
                }
                return routeValueDictionary;
            }

            public static RouteValueDictionary ObjectToDictionaryUncached(object value)
            {
                RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
                if (value != null)
                {
                    foreach (PropertyHelper property in PropertyHelper.GetProperties(value))
                        routeValueDictionary.Add(property.Name, property.GetValue(value));
                }
                return routeValueDictionary;
            }

            public static void AddAnonymousObjectToDictionary(
              IDictionary<string, object> dictionary,
              object value)
            {
                foreach (KeyValuePair<string, object> keyValuePair in TypeHelper.ObjectToDictionary(value))
                    dictionary.Add(keyValuePair);
            }

            public static bool IsAnonymousType(Type type)
            {
                if (type == (Type)null)
                    throw new ArgumentNullException(nameof(type));
                if (Attribute.IsDefined((MemberInfo)type, typeof(CompilerGeneratedAttribute), false) && type.IsGenericType && type.Name.Contains("AnonymousType") && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) || type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase)))
                    return (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
                return false;
            }
        }

        
    }
    public static class ViewContextExtensions
    {
        private static readonly object _lastFormNumKey = new object();
        internal static string FormIdGenerator(this ViewContext viewContext)
        {
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, "form{0}", (object)IncrementFormCount(viewContext.HttpContext.Items));
        }
        private static int IncrementFormCount(IDictionary items)
        {
            object obj = items[_lastFormNumKey];
            int num = obj != null ? (int)obj + 1 : 0;
            items[_lastFormNumKey] = (object)num;
            return num;
           
        }

        internal static MvcHtmlString ToMvcHtmlString(
      this TagBuilder tagBuilder,
      TagRenderMode renderMode)
        {
            return new MvcHtmlString(tagBuilder.ToString(renderMode));
        }

        internal static string ToJavascriptString(this AjaxOptions ajaxOptions)
        {
            StringBuilder stringBuilder = new StringBuilder("{");
            stringBuilder.AppendFormat((IFormatProvider)CultureInfo.InvariantCulture, " insertionMode: {0},", (object)ajaxOptions.InsertionModeString());
            stringBuilder.Append(PropertyStringIfSpecified("confirm", ajaxOptions.Confirm));
            stringBuilder.Append(PropertyStringIfSpecified("httpMethod", ajaxOptions.HttpMethod));
            stringBuilder.Append(PropertyStringIfSpecified("loadingElementId", ajaxOptions.LoadingElementId));
            stringBuilder.Append(PropertyStringIfSpecified("updateTargetId", ajaxOptions.UpdateTargetId));
            stringBuilder.Append(PropertyStringIfSpecified("url", ajaxOptions.Url));
            stringBuilder.Append(EventStringIfSpecified("onBegin", ajaxOptions.OnBegin));
            stringBuilder.Append(EventStringIfSpecified("onComplete", ajaxOptions.OnComplete));
            stringBuilder.Append(EventStringIfSpecified("onFailure", ajaxOptions.OnFailure));
            stringBuilder.Append(EventStringIfSpecified("onSuccess", ajaxOptions.OnSuccess));
            --stringBuilder.Length;
            stringBuilder.Append(" }");
            return stringBuilder.ToString();
        }

        internal static string InsertionModeString(this AjaxOptions ajaxOptions)
        {
            switch (ajaxOptions.InsertionMode)
            {
                case InsertionMode.Replace:
                    return "Sys.Mvc.InsertionMode.replace";
                case InsertionMode.InsertBefore:
                    return "Sys.Mvc.InsertionMode.insertBefore";
                case InsertionMode.InsertAfter:
                    return "Sys.Mvc.InsertionMode.insertAfter";
                default:
                    return ((int)ajaxOptions.InsertionMode).ToString((IFormatProvider)CultureInfo.InvariantCulture);
            }
        }
        private static string PropertyStringIfSpecified(string propertyName, string propertyValue)
        {
            if (string.IsNullOrEmpty(propertyValue))
                return string.Empty;
            string str = propertyValue.Replace("'", "\\'");
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, " {0}: '{1}',", (object)propertyName, (object)str);
        }
        private static string EventStringIfSpecified(string propertyName, string handler)
        {
            if (string.IsNullOrEmpty(handler))
                return string.Empty;
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, " {0}: Function.createDelegate(this, {1}),", (object)propertyName, (object)handler.ToString());
        }
    }
}