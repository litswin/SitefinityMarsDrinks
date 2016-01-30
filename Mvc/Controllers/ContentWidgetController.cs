using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using SitefinityWebApp.Mvc.Models;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Modules.GenericContent;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "ContentWidget", Title = "ContentWidget", SectionName = "MvcWidgets")]
    [Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesigner(typeof(WidgetDesigners.ContentWidget.ContentWidgetDesigner))]
    public class ContentWidgetController : Controller
    {
        /// <summary>
        /// Категория контентитемов для выборки
        /// </summary>
        [Category("String Properties")]
        public string Message { get; set; }

        /// <summary>
        /// This is the default Action.
        /// </summary>
        public ActionResult Index()
        {
            var model = new ContentWidgetModel();
            IEnumerable<ContentItem> contentItems = new List<ContentItem>();
            if (!string.IsNullOrWhiteSpace(Message))
            {
                contentItems = GetItems<ContentItem>(Message);
            }
            model.Message = Message;
            model.ContentItems = contentItems;

            return View(model);
        }

        private IEnumerable<T> GetItems<T>(string categoryName)
        {
            Guid taxonGuidId = GetCategoryIdByCategoryName(categoryName);
            if (taxonGuidId == Guid.Empty)
                return new List<T>();

            IManager manager = ManagerBase.GetMappedManager(typeof(T), "");
            ContentDataProviderBase contentProvider = manager.Provider as ContentDataProviderBase;
            return GetItems<T>(taxonGuidId, contentProvider);
        }

        private IEnumerable<T> GetItems<T>(Guid taxonGuidId, ContentDataProviderBase contentProvider)
        {
            TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
            ITaxon taxon = taxonomyManager.GetTaxon(taxonGuidId);
            TaxonomyPropertyDescriptor prop = TaxonomyManager.GetPropertyDescriptor(typeof (T), taxon);
            int? totalCount = 0;
            string filter = string.Format("Status = {0}", ContentLifecycleStatus.Live);
            IEnumerable items = contentProvider.GetItemsByTaxon(taxon.Id, prop.MetaField.IsSingleTaxon, prop.Name,
                typeof (T), filter, "Title asc", 0, 100, ref totalCount);
            return (from object item in items select (T) item).ToList();
        }

        /// <summary>
        /// Get the category ID by the category name
        /// </summary>
        /// <param name="categoryName">Name of the category. Must be an exact match</param>
        /// <returns>Guid of category, or empty guid if null</returns>
        private Guid GetCategoryIdByCategoryName(string categoryName)
        {
            Guid id = Guid.Empty;
            TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
            const string taxonomyName = "categories";
            HierarchicalTaxonomy taxonomy = taxonomyManager.GetTaxonomies<HierarchicalTaxonomy>().SingleOrDefault(t => t.Name.ToLower() == taxonomyName.ToLower());
            if (taxonomy == null)
                return id;

            HierarchicalTaxon taxonomyForCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().SingleOrDefault(t => t.Name.ToLower() == categoryName.ToLower());
            if (taxonomyForCategory != null)
            {
                id = taxonomyForCategory.Id;
            }
            return id;
        }
    }
}