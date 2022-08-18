using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using System.IO;
using System.Threading.Tasks;
using Talabat.APIs.Errors;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.APIs.Controllers
{
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService paymentService;
        private readonly ILogger<PaymentController> logger;
        private const string whSecret = "whsec_11290dd15feb1b5f1dcc7ca5b47a8e009e5d20d93255e96cc2c5c6d741aaf07c";

        public PaymentController(IPaymentService paymentService , ILogger<PaymentController> logger)
        {
            this.paymentService = paymentService;
            this.logger = logger;
        }
        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null) return BadRequest(new ApiResponse(400, "Problem with your basket"));

            return Ok(basket);
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], whSecret);

                PaymentIntent intent;
                Order order;
                switch (stripeEvent.Type)
                {
                    case Events.PaymentIntentSucceeded:
                        logger.LogInformation("Payment Succeded");
                        intent = (PaymentIntent)stripeEvent.Data.Object;
                        order = await paymentService.UpdateOrderPaymentSucceded(intent.Id);
                        break;
                    case Events.PaymentIntentPaymentFailed:
                        intent = (PaymentIntent)stripeEvent.Data.Object;
                        order = await paymentService.UpdateOrderPaymentFailed(intent.Id);
                        logger.LogInformation("Payment Failed" , order.Id);
                        logger.LogInformation("Payment Failed" , intent.Id);
                        break;
                }
                return new EmptyResult();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
