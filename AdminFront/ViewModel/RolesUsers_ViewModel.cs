namespace MIA.ViewModel
{
    /// <summary>
    /// Clase que contiene las propiedades para la vista de RolesUsers
    /// </summary>
    public class RolesUsers_ViewModel
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string NameRol { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
