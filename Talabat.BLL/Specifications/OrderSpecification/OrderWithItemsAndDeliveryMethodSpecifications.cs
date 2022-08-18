using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.APIs.Specifications;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.BLL.Specifications.OrderSpecification
{
    public class OrderWithItemsAndDeliveryMethodSpecifications : BaseSpecification<Order>
    {
        public OrderWithItemsAndDeliveryMethodSpecifications(string buyerEmail) : base(O => O.BuyerEmail == buyerEmail)
        {
            AddInclude(O => O.Items);
            AddInclude(O => O.DeliveryMethod);

            AddOrderByDecending(O => O.OrderDate);
        }

        public OrderWithItemsAndDeliveryMethodSpecifications(int orderId, string buyerEmail) :
                                                         base(O => (O.BuyerEmail == buyerEmail && O.Id == orderId))
        {
            AddInclude(O => O.Items);
            AddInclude(O => O.DeliveryMethod);
        }
    }
}
