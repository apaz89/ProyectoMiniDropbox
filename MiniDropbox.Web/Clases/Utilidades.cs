using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Iesi.Collections;
using MiniDropbox.Domain;

namespace MiniDropbox.Web.Clases
{
    public class Utilidades
    {
        static private Account _account;

        public  Account _AccountActual { get { return _account; } set { _account = value; } }
    }
}