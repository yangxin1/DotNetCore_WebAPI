using DotNet_Core_API_SwaggerDemo1.IDAL;
using DotNet_Core_API_SwaggerDemo1.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDAL
{
    public interface IHomeDAL:IDBHelper<User>
    {
    }
}
