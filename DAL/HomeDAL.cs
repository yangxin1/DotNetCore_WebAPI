using DotNet_Core_API_SwaggerDemo1.DAL;
using DotNet_Core_API_SwaggerDemo1.Model;
using IDAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL
{
    public class HomeDAL:DBHelper<User>,IHomeDAL
    {
    }
}
