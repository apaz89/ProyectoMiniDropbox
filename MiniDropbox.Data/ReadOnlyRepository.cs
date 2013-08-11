using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MiniDropbox.Domain;
using MiniDropbox.Domain.Entities;
using MiniDropbox.Domain.Services;
using NHibernate;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq;

namespace MiniDropbox.Data
{
    public class ReadOnlyRepository : IReadOnlyRepository
    {
        private readonly ISession _session;

        public ReadOnlyRepository(ISession session)
        {
            _session = session;
        }

        public T First<T>(Expression<Func<T, bool>> query) where T : class, IEntity
        {
            T firstOrDefault = _session.Query<T>().FirstOrDefault(query);
            return firstOrDefault;
        }

        public T GetById<T>(long id) where T : class, IEntity
        {
            var item = _session.Get<T>(id);
            return item;

        }

        public IQueryable<T> Query<T>(Expression<Func<T, bool>> expression) where T : class, IEntity
        {
            return _session.Query<T>().Where(expression);
        }

        public List<T> AllItemsRead<T>()
        {
            var tablaQuery =
                (from tabla in _session.Query<T>() select tabla).ToList();
            return tablaQuery;
        }

        public Account GetAccountEmail(string email)
        {
            var item = (from tabla in _session.Query<Account>()
                                 where tabla.Email == email
                                 select tabla).FirstOrDefault();
            return item;
        }

        public Account GetAccountwithToken(string token)
        {
            var _Token = (from tabla in _session.Query<TransaccionesUrl>() where tabla.token==token select tabla ).FirstOrDefault();
            if (_Token != null)
            {
                return GetById<Account>(_Token.Account_Id);
            }
            return new Account();
        }
    }   
}
