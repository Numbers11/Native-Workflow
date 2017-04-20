﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.Infrastructure;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Html;
using NeuroSystem.Workflow.UserData.UI.Html.ASP.UI.Html.Mvc.Extensions;
using NeuroSystem.Workflow.UserData.UI.Html.ASP.UI.Html.Mvc.Widgets;
using NeuroSystem.Workflow.UserData.UI.Html.ASP.UI.Html.Version2.TestViews;
using NeuroSystem.Workflow.UserData.UI.Html.ASP.UI.Html.Version2.Widgets;
using NeuroSystem.Workflow.UserData.UI.Html.Mvc.Fluent;
using NeuroSystem.Workflow.UserData.UI.Html.Mvc.UI;
using NeuroSystem.Workflow.UserData.UI.Html.Version1.DataAnnotations;

namespace NeuroSystem.Workflow.UserData.UI.Html.ASP.UI.Html.Version2.Extensions
{
    public static class HtmlHelperExtension
    {
        public static WidgetFactory Ns(this System.Web.Mvc.HtmlHelper helper)
        {
            return new WidgetFactory(helper);
        }

        public static object Ns<TModel>(this HtmlHelper<TModel> helper)
        {
            var viewContext = helper.ViewContext;
            var initializer = DI.Current.Resolve<IJavaScriptInitializer>();
            var urlGenerator = DI.Current.Resolve<IUrlGenerator>();
            var viewData = helper.ViewData;

            //var model = new PracownikTest();
            //model.Imie = "Jan";
            //model.Nazwisko = "Kowalski";

            //var html = new WidgetFactory();
            //var panelGlwony = html.Panel();

            //var g1 = html.Panel();
            //g1.Class("form-horizontal form-widgets col-sm-6");

            //var tb = html.TextBox<string>();
            //tb.Name("Imie");
            //g1.AddItem(tb);

            //var b2 = html.TextBox<string>();
            //b2.Name("Nazwa");
            //g1.AddItem(b2);
            //panelGlwony.AddItem(g1);

            //var dp = html.DatePicker();
            //dp.Value(DateTime.Now);
            //dp.Name("Data");
            //g1.AddItem(dp);

            //var cb = html.ComboBox();
            //cb.Name("cb");

            //cb.DataSource(ds => ds.Read("PracownikTestModel_Read", "Proces")
            //    );
            //g1.AddItem(cb);


            //var grid = html.Grid<PracownikTest>();
            //grid.Name("grid");

            //grid.DataSource(ds => ds
            //    .Ajax()
            //    .Model(m => m.Id("Id"))
            //    .Read("PracownikTestModel_Read", "Proces")
            //    );
            //panelGlwony.AddItem(grid);

            //var kontrolka = panelGlwony.ToComponent().ToKendoWidget(viewContext, initializer, helper.ViewData, urlGenerator);

            //return new MvcHtmlString(kontrolka.ToHtmlString());


            var model = new PracownikTest();
            model.Imie = "Jan";
            model.Nazwisko = "Kowalski";

            var html = new WidgetFactory();
            var panelGlwony = html.Panel();
            
            var visibleProperty = new List<VisibleProperty>();
            var type = model.GetType();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var displays = propertyInfo.GetCustomAttributes(typeof(DataFormViewAttribute));
                if (displays.Any())
                {
                    var display = displays.First() as DataFormViewAttribute;
                    visibleProperty.Add(new VisibleProperty() { DataFormView = display, PropertyInfo = propertyInfo });
                }
            }

            visibleProperty = visibleProperty.Where(v => v.DataFormView.GroupName == null).ToList();

            var nsGrupa = generujGrupe(visibleProperty, viewContext, initializer, viewData, urlGenerator);
            var htmlstr = nsGrupa.ToHtmlString();
            return new MvcHtmlString(htmlstr);
        }

        public class VisibleProperty
        {
            public PropertyInfo PropertyInfo { get; set; }
            public GridViewAttribute ListView { get; set; }
            public DataFormViewAttribute DataFormView { get; set; }

        }

        public static object DataForm<TModel>(this HtmlHelper<TModel> helper, TModel model)
        {
            var viewContext = helper.ViewContext;
            var initializer = DI.Current.Resolve<IJavaScriptInitializer>();
            var urlGenerator = DI.Current.Resolve<IUrlGenerator>();
            var viewData = helper.ViewData;
            

            var biznesObject = new PracownikTest();
            var viewPanel = new Panel();
            var view = new NsPanel(viewPanel, viewContext, initializer, viewData, urlGenerator);

            var visibleProperty = new List<VisibleProperty>();

            var type = biznesObject.GetType();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var displays = propertyInfo.GetCustomAttributes(typeof(DataFormViewAttribute));
                if (displays.Any())
                {
                    var display = displays.First() as DataFormViewAttribute;
                    visibleProperty.Add(new VisibleProperty() { DataFormView = display, PropertyInfo = propertyInfo });
                }
            }

            var tabsName = visibleProperty.Where(w => w.DataFormView.TabName != null)
               .Select(w => w.DataFormView.TabName).Distinct().ToList();
            if (tabsName.Count > 1)
            {
                //var panel = view.AddPanel();
                var tabsControl = helper.Kendo().TabStrip().ToComponent();
                foreach (var tabName in tabsName)
                {
                    //tabsControl.Items.Add();
                    var tab = new TabStripItem();
                    var tabWidgets = visibleProperty.Where(w => w.DataFormView.TabName == tabName);
                    generateGroups(tabWidgets, tab);
                }

            }
            else
            {
                ////visibleProperty = visibleProperty.OrderBy(p => p.Order).ToList();
                //var df = view.AddDataForm();
                //if (title != null)
                //{
                //    df.AddLabel(title);
                //}
                //if (description != null)
                //{
                //    df.AddLabel(description);
                //}

                //view.ActiveDataForm = df;
                //generateGroups(visibleProperty, df);
            }

            return new MvcHtmlString("");
        }

        private static void generateGroups(IEnumerable<VisibleProperty> tabWidgets, TabStripItem tab)
        {
            var groupsName = tabWidgets.Select(w => w.DataFormView.GroupName).Distinct().ToList();
            foreach (var groupName in groupsName)
            {
                //var groupWidgets = tabWidgets.Where(w => w.DataFormView.GroupName == groupName);
                //var panel = new Panel();
                //var groupPanel = new NsPanel(panel, viewContext, initializer, viewData, urlGenerator);
                ////groupPanel.Label(groupName);
                ////groupPanel.Width("49%");
                ////groupPanel.Float(EnumPanelFloat.Left);
                //var df = groupPanel.AddDataForm();
                //foreach (var groupWidget in groupWidgets)
                //{
                //    if (groupWidget.DataFormView.RepositoryType != null)
                //    {
                //        var cb = df.AddComboBox(groupWidget.PropertyInfo.Name).LoadOnDemand(true);
                //        cb.DataSource(GetDataSourceByType(groupWidget.DataFormView.RepositoryType));
                //        cb.Label(groupWidget.PropertyInfo.Name);
                //    }
                //    else
                //    {
                //        df.AddField(groupWidget.PropertyInfo).Height = groupWidget.DataFormView.Height;
                //    }
                //}
            }
            //tab.AddPanel(p => p.Clear(EnumPanelClear.Both));
        }

        public static NsPanel generujGrupe(IEnumerable<VisibleProperty> tabWidgets
            , ViewContext viewContext, IJavaScriptInitializer initializer,
            ViewDataDictionary viewData, IUrlGenerator urlGenerator)
        {
            var nsgrupa = new NsPanel(new Panel(),viewContext, initializer, viewData, urlGenerator );
            nsgrupa.Class("form-horizontal form-widgets col-sm-6");

            foreach (var visibleProperty in tabWidgets)
            {
                //blok
                var blok = new NsPanel(new Panel(), viewContext, initializer, viewData, urlGenerator);
                blok.Class("form-group");
                var label = new NsLabel(viewContext);
                label.Content = visibleProperty.PropertyInfo.Name;
                blok.AddItem(label);
                var div = new Panel();
                div.Class("col-sm-8 col-md-8");
                blok.Panel.Items.Add(div);
                var tb = new UserData.UI.Html.Mvc.UI.TextBox<string>();
                tb.Name = visibleProperty.PropertyInfo.Name;
                div.Items.Add(tb);

                nsgrupa.AddItem(blok);
            }

            return nsgrupa;
        }
    }
}
