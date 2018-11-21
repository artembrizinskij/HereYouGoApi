using System.Collections.Generic;

namespace Infastructure.Extensions
{
    public static class AutoMapExtensions
    {
        public static TDest MapTo<TDest>(this object src)
        {
            return (TDest)AutoMapper.Mapper.Map(src, src.GetType(), typeof(TDest));
        }

        public static IEnumerable<TDest> MapEachTo<TDest>(this IEnumerable<object> objects)
        {
            var mapList = new List<TDest>();
            foreach (var x in objects)
                mapList.Add((TDest)AutoMapper.Mapper.Map(x, x.GetType(), typeof(TDest)));
            return mapList;
        }
    }
}
