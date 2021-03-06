﻿using System;

namespace NeuroSystem.Workflow.UserData.UI.Html.Version1.DataSources
{
    /// <summary>
    /// Obiekt reprezentujący zródło danych
    /// </summary>
    public class DataSourceBase
    {
        /// <summary>
        /// Zwraca wszystekie dane
        /// </summary>
        /// <param name="filtr"></param>
        /// <returns></returns>
        public virtual object GetAll(string filtr = null)
        {
            throw new NotImplementedException();
        }

        public virtual object GetData(long start, long cout, string sort, string filtr,
            out long virtualItemsCout)
        {
            throw new NotImplementedException();
        }

        public virtual object GetObjectById(string id, bool isDisconectedToDb = false)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(object objectToSave)
        {
            throw new NotImplementedException();
        }

        public virtual void Add(object objectToAdd)
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public virtual object CreateNewObject()
        {
            throw new NotImplementedException();
        }
    }
}
