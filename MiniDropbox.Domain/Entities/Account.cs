using System.Collections.Generic;
using MiniDropbox.Domain.Entities;

namespace MiniDropbox.Domain
{
    public class Account : IEntity
    {
        private readonly IList<Role> _roles = new List<Role>();

        public virtual IEnumerable<Role> Roles
        {
            get { return _roles; }
        }

        public virtual void AddRole(Role role)
        {
            if (!_roles.Contains(role))
            {
                _roles.Add(role);
            }
        }
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Apellido { get; set; }
        public virtual long Consumo { get; set; }
        public virtual long EspacioAsignado { get; set; } ////no debe ser mmenor que el consumo.
        public virtual string Tipo { get; set; }//administrador, cliente
        public virtual bool Estado { get; set; }
        public virtual IList<File> Files { get; set; }
        public virtual IList<ReferredUsersList> ReferredUsers { get; set; }
        public virtual IList<PaquetesPremium> PaquetesPremiums { get; set; }
        public virtual IList<TransaccionesUrl> TransaccionesUrls { get; set; }
        public virtual string HashConfirmation { get; set; }
        public virtual bool IsConfirm { get; set; }
        
    }
}  