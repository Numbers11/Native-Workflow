﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroSystem.VirtualMachine.Core.Attributes;
using NeuroSystem.Workflow.Core.Extensions;
using NeuroSystem.Workflow.UserData.UI.Html.DataSources;
using NeuroSystem.Workflow.UserData.UI.Html.Fluent.Views;

namespace NeuroSystem.Workflow.Core.Process.ProcessWithUI.Html
{
    public class EditProcess<T> : HtmlProcessBase
    {
        public EditProcess()
        {
            UserActions = new List<string>();
        }

        #region properce
        public List<string> UserActions { get; set; }

        public Func<string, object> PerformUserAction;
        #endregion


        public override object Start()
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

        [Interpret]
        public void ShowMessage(string text, string endText = "Zakończono proces")
        {
            var view=  CreateDataFormView(new object(), text);
            view.AddAction("Ok");
            view.AddAction("Zamknij");

            var wynikEdycji = ShowView(view);
            if (wynikEdycji.ActionName == "Zamknij")
            {
                EndProcess(endText);
            }
        }

        public virtual object InvokeUserAction(string actionName)
        {
            if (PerformUserAction != null)
            {
                return PerformUserAction(actionName);
            }
            return null;
        }

        public virtual DataSourceBase GetDataSource()
        {
            return null;
        }

        public virtual void UpdateObject(T obiekt)
        {
            var ds = GetDataSource();
            ds.Update(obiekt);
        }

        public virtual DataFormFactory<T> CreateEditView(T obj)
        {
            return CreateDataFormView(obj, "Edycja obiektu", this.GetType().GetClassDescription());
        }
    }
}
