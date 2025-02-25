using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escola
{
    class BancoConexao
    {
        sqlConnection conexao = new sqlConnection();
        string stringConexao = "Data Source=DESKTOP-7VJGJ8V;Initial Catalog=BDEscolaADO;Integrated Security=True";
    }

    internal class sqlConnection
    {
        public sqlConnection()
        {
        }
    }
}
