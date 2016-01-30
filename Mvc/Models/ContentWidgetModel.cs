using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.GenericContent.Model;

namespace SitefinityWebApp.Mvc.Models
{
    public class ContentWidgetModel
    {
        /// <summary>
        /// Категория контентитемов для выборки
        /// </summary>
        public string Message { get; set; }

        public IEnumerable<ContentItem> ContentItems { get; set; }
    }
}