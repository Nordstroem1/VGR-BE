using Domain.Models;

namespace Application.Dtos.ReponsObjects
{
    public class OrderReponse
    {
        public DateTime OrderTimeStamp { get; set; }
        public Article Article { get; set; }
    }
}
