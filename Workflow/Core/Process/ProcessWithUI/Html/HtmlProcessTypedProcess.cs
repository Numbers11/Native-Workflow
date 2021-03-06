﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroSystem.VirtualMachine.Core.Attributes;
using NeuroSystem.Workflow.Core.Extensions;
using NeuroSystem.Workflow.UserData.UI.Html.Version1.DataSources;
using NeuroSystem.Workflow.UserData.UI.Html.Version1.Fluent.Views;
using NeuroSystem.Workflow.UserData.UI.Html.Version1.Widgets.ItemsWidgets;

namespace NeuroSystem.Workflow.Core.Process.ProcessWithUI.Html
{
    /// <summary>
    /// Proces typowany - obsługujący jakiś proces dla danego typu (np. lista lub edycja)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HtmlProcessBase<T> : HtmlProcessBase
    {
        public override object Start()
        {
            if (ProcesInput is T)
            {
                return PodProcesEdycji();
            }
            else
            {
                return PodProcesListy();
            }
        }

        #region Propercje

        public bool AllowEditingList { get; set; }

        #endregion

        #region Pod procesy

        [Interpret]
        public object PodProcesEdycji()
        {
            if (CanExecute())
            {
                var obiekt = (T)ProcesInput;
                var widokEdycji = CreateEditView(obiekt);
                widokEdycji.AddAction("Zapisz");
                widokEdycji.AddAction("Anuluj");

                //dodaje akce użytkownika
                foreach (var userAction in UserActions)
                {
                    widokEdycji.AddAction(userAction);
                }

                var wynikEdycji = ShowView(widokEdycji);
                if (wynikEdycji.ActionName == "Zapisz")
                {
                    UpdateObject(obiekt);
                    return "Wykonano edycję obiektu " + obiekt;
                }
                else if (wynikEdycji.ActionName == "Anuluj")
                {
                    return "Anulowano edycję " + obiekt;
                }

                return InvokeUserAction(wynikEdycji.ActionName);
            }
            else
            {
                return "Brak możliwości wykonania procesu";
            }
        }

        [Interpret]
        public object PodProcesDodawaniaNowegoObiektu()
        {
            if (CanExecute())
            {
                var obiekt = CreateNewObject();
                var widokEdycji = CreateEditView(obiekt);
                widokEdycji.AddAction("Dodaj");
                widokEdycji.AddAction("Anuluj");

                var wynikEdycji = ShowView(widokEdycji);
                if (wynikEdycji.ActionName == "Dodaj")
                {
                    UpdateObject(obiekt);
                    return "Dodano obiekt " + obiekt;
                }
                else
                {
                    return "Anulowano dodawanie obiektu " + obiekt;
                }
            }
            else
            {
                return "Brak możliwości wykonania procesu";
            }
        }

        [Interpret]
        public object PodProcesListy()
        {
            if (CanExecute())
            {
                while (true)
                {
                    var widokListy = CreateGrid<T>(
                        "Lista obiektów typu '" + typeof(T).Name + "'",
                        typeof(T).GetClassDescription());
                    widokListy.DataSource(GetProcessDataSource());
                    widokListy.Grid.AllowEditing(AllowEditingList);
                    generateGridMenu(widokListy);

                    var wynikListy = ShowView(widokListy);
                    var grid = wynikListy.Panel.GetWidgetByType<GridView>();
                    var zaznaczoneId = new List<string>();

                    if (grid.SelectedIds != null && grid.SelectedIds.Any())
                    {
                        foreach (var gridSelectedId in grid.SelectedIds)
                        {
                            zaznaczoneId.Add(gridSelectedId);
                        }
                    }

                    if (wynikListy.ActionName == "Dodaj nowy")
                    {
                        AddNewObject();
                    }
                    else
                    {
                        var zaznaczonyObiekt = grid.SelectedValue?.ToString();
                        if (zaznaczonyObiekt == null)
                        {
                            //continue;
                        }
                        else if (wynikListy.ActionName == "Edytuj")
                        {
                            Edit(zaznaczonyObiekt);
                        }
                        else if (wynikListy.ActionName == "Usuń")
                        {
                            DeleteFromGrid(grid);
                        }
                        else if (wynikListy.ActionName == "Zamknij")
                        {
                            EndProcess("Zakończono listę " + nameof(T));
                        }
                        else
                        {
                            InvokeUserActionForList(wynikListy.ActionName, zaznaczoneId);
                        }
                    }
                }
            }
            else
            {
                return "Brak możliwości wykonania procesu";
            }
        }

        [Interpret]
        public void Edit(string selectedObjectId)
        {
            var obiekt = GetObjectById(selectedObjectId);
            var widokEdycji = CreateEditView(obiekt);
            widokEdycji.AddAction("Zapisz");
            widokEdycji.AddAction("Anuluj");

            var wynikEdycji = ShowView(widokEdycji);
            if (wynikEdycji.ActionName == "Zapisz")
            {
                obiekt = (T)wynikEdycji.DataContext;

                UpdateObject(obiekt);
                ShowMessage("Zapisano zmiany w " + obiekt);
            }
        }

        [Interpret]
        public object PodProcesTreeView()
        {
            while (true)
            {
                var widokTreeView = new ViewFactory<T>();
                var df = widokTreeView.AddDataForm();
                var tv = df.AddTreeView();
                tv.DataSource(GetProcessDataSource());
                tv.TreeView.DataIdField = "Id";
                tv.TreeView.DataValueField = "Id";
                tv.TreeView.DataFieldParentId = "RodzicId";

                generateGridMenu(widokTreeView);

                var wynikTreeView = ShowView(widokTreeView);

                if (wynikTreeView.ActionName == "Dodaj nowy")
                {
                    AddNewObject();
                }
                else
                {
                    var grid = wynikTreeView.Panel.GetWidgetByType<TreeView>();
                    var zaznaczonyObiekt = grid.SelectedValue?.ToString();
                    if (zaznaczonyObiekt == null)
                    {
                        //continue;
                    }
                    else if (wynikTreeView.ActionName == "Edytuj")
                    {
                        Edit(zaznaczonyObiekt);
                    }
                    else if (wynikTreeView.ActionName == "Usuń")
                    {
                        DeleteObject(zaznaczonyObiekt);
                    }
                    else if (wynikTreeView.ActionName == "Zamknij")
                    {
                        EndProcess("Zakończono listę " + nameof(T));
                    }
                }
            }
        }


        [Interpret]
        public void AddNewObject()
        {
            var obiekt = CreateNewObject();
            var widokEdycji = CreateEditView(obiekt);
            widokEdycji.AddAction("Dodaj");
            widokEdycji.AddAction("Anuluj");

            var wynikEdycji = ShowView(widokEdycji);
            if (wynikEdycji.ActionName == "Dodaj")
            {
                obiekt = (T)wynikEdycji.DataContext;

                AddObject(obiekt);
                wynikEdycji.Panel?.SetDataContext(null);
                obiekt = default(T); //null'uje obiekt żeby go nie serializować
                ShowMessage("Dodano nowy obiektu " + obiekt);
            }
        }

        [Interpret]
        public void DeleteFromGrid(GridView grid)
        {
            DeleteObjects(grid);
            ShowMessage("Usunięto obiekt o Id " + grid.SelectedValue?.ToString());
        }

        #endregion


        #region Funkcje dla typu T (Źródłą danych i UI)

        #region Źródło danych i DAL

        /// <summary>
        /// Zwraca główne źródło danych procesu
        /// </summary>
        /// <returns></returns>
        protected virtual DataSourceBase GetProcessDataSource()
        {
            return null;
        }

        public virtual void UpdateObject(T obiekt)
        {
            var ds = GetProcessDataSource();
            ds.Update(obiekt);
        }

        public virtual string DeleteObjects(GridView grid)
        {
            string zaznaczoneObiekty = "";
            if (grid.SelectedIds != null && grid.SelectedIds.Any())
            {
                foreach (var gridSelectedId in grid.SelectedIds)
                {
                    zaznaczoneObiekty += gridSelectedId + ";";
                    DeleteObject(gridSelectedId);
                }
            }
            else
            {
                var zaznaczonyObiekt = grid.SelectedValue?.ToString();
                zaznaczoneObiekty += zaznaczonyObiekt + ";";
                DeleteObject(zaznaczonyObiekt);
            }
            return zaznaczoneObiekty;
        }

        public virtual void DeleteObject(string id)
        {
            var ds = GetProcessDataSource();
            ds.DeleteById(id);
        }

        public virtual T GetObjectById(string id)
        {
            var ds = GetProcessDataSource();
            return (T)ds.GetObjectById(id, true);
        }

        public virtual T CreateNewObject()
        {
            var ds = GetProcessDataSource();
            return (T)ds.CreateNewObject();
        }

        public virtual void AddObject(object newObject)
        {
            var ds = GetProcessDataSource();
            ds.Add(newObject);
        }

        #endregion

        #region UI

        /// <summary>
        /// Tworzy widok edycji
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual DataFormFactory<T> CreateEditView(T obj)
        {
            var editView = CreateDataForm(obj, "Edycja obiektu", this.GetType().GetClassDescription());
            return editView;
        }

        /// <summary>
        /// Tworzy widok grida/listy
        /// </summary>
        /// <returns></returns>
        public virtual ListViewFactory<T> CreateGridView()
        {
            var widokListy = CreateGrid<T>(
                    "Lista obiektów typu '" + typeof(T).Name + "'",
                    typeof(T).GetClassDescription());
            widokListy.DataSource(GetProcessDataSource());
            generateGridMenu(widokListy);

            return widokListy;
        }

        /// <summary>
        /// Dodaje menu użytkownika do widoku
        /// </summary>
        /// <param name="widokListy"></param>
        protected void generateUserMenu(ViewFactory<T> widokListy)
        {
            //dodaje akce użytkownika
            foreach (var userAction in UserActions)
            {
                widokListy.AddAction(userAction);
            }
        }

        /// <summary>
        /// Dodaje menu do widoku grida
        /// </summary>
        /// <param name="widokListy"></param>
        protected void generateGridMenu(ViewFactory<T> widokListy)
        {
            widokListy.AddAction("Edytuj");
            widokListy.AddAction("Dodaj nowy");
            widokListy.AddAction("Usuń");
            widokListy.AddAction("Zamknij");

            generateUserMenu(widokListy);
        }

        #endregion

        #endregion

    }
}
