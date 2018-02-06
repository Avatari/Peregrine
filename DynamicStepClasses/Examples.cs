using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TaskMgrTypes;
using TaskMgrTypes.Constants;

namespace DynamicStepClasses.Examples
{

    public class GetPriorDayStockPrices : IStep
    {
        public OutputValues Execute(InputValues input)
        {
            OutputValues output = new OutputValues();

            // implement logic to get prior day's stock prices


            //output.PostExecutionDecision = PostExecutionDecision.;  // based on execution return dicision for next steps
            // output.Values = ;    // if any values need to pass between steps

            return output;
        }
    }

    public class IfReadyCalculateReturns : IStep
    {
        public OutputValues Execute(InputValues input)
        {
            OutputValues output = new OutputValues();

            // if want to wait for somethig set ideled to true
            //output.IsIdled = true;
            //else
            // do execution

            output.PostExecutionDecision = PostExecutionDecision.Ok;  // based on execution return dicision for next steps
            // output.Values = ;    // if any values need to pass between steps

            return output;
        }
    }

    public class PublishReturns : IStep
    {
        public OutputValues Execute(InputValues input)
        {
            OutputValues output = new OutputValues();

            // logic to publish returns

            //output.PostExecutionDecision = PostExecutionDecision.;  // based on execution return decision for next steps
            // output.Values = ;    // if any values need to pass between steps

            return output;
        }
    }

    public class GetProduct : IStep
    {
        public OutputValues Execute(InputValues input)
        {
            OutputValues output = new OutputValues();

            // step logic
            var prod = new Product { Name = "Test Product", Price = 100 };

            // based on execution return decision for next steps
            output.PostExecutionDecision = PostExecutionDecision.Ok;

            output.Values = JsonConvert.SerializeObject(prod);

            return output;
        }
    }


    public class ChangeAndReturnProduct : IStep
    {

        public OutputValues Execute(InputValues input)
        {
            Product inputProd = JsonConvert.DeserializeObject<Product>(input.Values);

            inputProd.Name += " - Modified";
            inputProd.Price *= 1.25;

            OutputValues output = new OutputValues();

            output.Values = JsonConvert.SerializeObject(inputProd);

            return output;
        }
    }

    public class ThrowError : IStep
    {
        public OutputValues Execute(InputValues input)
        {
            OutputValues output = new OutputValues();
            output.PostExecutionDecision = PostExecutionDecision.Yes;

            try
            {
                throw new Exception("A custom message for an application specific exception");
            }
            catch (Exception ex)
            {
                output.FailureInfo = new FailureInfo { Message = ex.Message };
                output.PostExecutionDecision = PostExecutionDecision.Error;
            }

            return output;
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public double Price { get; set; }

    }

}
