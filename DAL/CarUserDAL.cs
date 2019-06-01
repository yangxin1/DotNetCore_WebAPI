using DotNet_Core_API_SwaggerDemo1.IDAL;  
using DotNet_Core_API_SwaggerDemo1.Models;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DotNet_Core_API_SwaggerDemo1.DAL
{
    /// <summary>
    /// 车辆用户DAL
    /// </summary>
    public class CarUserDAL : DBHelper<CarUser>, ICarUserDAL
    {
    }
}
