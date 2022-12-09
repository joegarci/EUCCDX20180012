using MIA.Model;
using MIA.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;

namespace MIA.Helpers
{
    public class CommonFunctions
    {
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }
        public User GetUser(object obj)
        {
            using (ModelDB modeldb = new ModelDB())
            {
                return modeldb.Users.Where(q => q.NameUser == (string)obj).FirstOrDefault();
            }
        }
        public List<Assistants> GetAssitant()
        {
            using (ModelDB modeldb = new ModelDB())
            {
                return (from a in modeldb.Assistants
                        join b in modeldb.UsersAssistants on a.IdAssistant equals b.IdAssistant
                        join c in modeldb.Users on b.IdUser equals c.IdUser
                        where c.NameUser.ToUpper() == Environment.UserName.ToUpper() && b.ActiveUserAssistant == true
                        select a).ToList();
            }
        }
        public List<Check_ViewModel> GetStates(int Id)
        {
            List<Check_ViewModel> ListState = new List<Check_ViewModel>();
            using (ModelDB modeldb = new ModelDB())
            {
                ListState.AddRange(from item in modeldb.States.Where(P => P.IdAssistant == Id).ToList()
                                   let NewState = new Check_ViewModel()
                                   {
                                       NameState = item.NameState,
                                       IdState = item.IdState,
                                       IsSelect = false
                                   }
                                   select NewState);
            }
            return ListState;
        }
        public List<string> GetNextStates(int Id)
        {
            using (ModelDB modeldb = new ModelDB())
            {
                int id = modeldb.Assistants.Where(P => P.IdAssistant == Id).FirstOrDefault().IdAssistant;
                return modeldb.States.Where(P => P.IdAssistant == id).Select(P => P.NameState).ToList();
            }
        }
        public string TypeUser(string NameUser)
        {
            using (ModelDB modeldb = new ModelDB())
            {
                return (from a in modeldb.Users
                        join b in modeldb.TypeUsers on a.IdType equals b.IdType
                        where a.NameUser == NameUser
                        select b.CodUserType).FirstOrDefault();
            }
        }
        public List<UsersTypes> GetTypeUsers()
        {
            using (ModelDB modeldb = new ModelDB())
            {
                return modeldb.TypeUsers.Where(P => P.CodUserType != "CodRpa").ToList();
            }
        }
        public bool IsValid(ModelDB modeldb)
        {
            bool isValid = false;
            var result = modeldb.GetValidationErrors().ToList();
            if (result.Count == 0)
            {
                isValid = true;
            }
            else
            {
                StringBuilder sbErrors = new StringBuilder();
                sbErrors.AppendLine("Se han presentando los siguientes Errores:");
                sbErrors.AppendLine("");
                foreach (var item in result.FirstOrDefault().ValidationErrors)
                {
                    sbErrors.AppendFormat("- {0}\n", item.ErrorMessage);
                }
                MessageBox.Show(sbErrors.ToString(), "Mensaje de Validación", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return isValid;
        }
    }
}
