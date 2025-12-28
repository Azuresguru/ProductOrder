using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContract;
using DataAcessLayer.Entities;
using FluentValidation;

namespace productMicroservice.API
{
    public static class ProductionApiEndpoints
    {

        public static IEndpointRouteBuilder MapProductionApiEndpoints(this IEndpointRouteBuilder app)
        {

            //get /api/products
            //
            app.MapGet("/api/products",
                async (IProductsService productService) =>
                {
                    IEnumerable<ProductResponse>? products = await productService.GetProducts();
                    return Results.Ok(products);
                });

            //get /api/products/search/productId/0000000000000
            //
            app.MapGet("/api/products/search/{productId:guid}",
                async (IProductsService productService,Guid productId) =>
                {
                    ProductResponse? products = await productService.GetProductByCondition(x=>x.ProductId==productId);
                    return Results.Ok(products);
                });
            app.MapGet("/api/products/search/{searchString}",
              async (IProductsService productService, string searchString) =>
              {
                  IEnumerable<ProductResponse>? productsByName = await productService.GetProductsByCondition(x=>x.ProductName != null && x.ProductName.Contains(searchString,StringComparison.OrdinalIgnoreCase));
                 
                  IEnumerable<ProductResponse>? productsbyCategory = await productService.GetProductsByCondition(x => x.Category != null && x.Category.Contains(searchString, StringComparison.OrdinalIgnoreCase));

                  var products = productsByName.Union(productsbyCategory);
                  return Results.Ok(products);
              });

            app.MapPost("/api/products",
               async (IProductsService productService, ProductAddRequest productAddRequest,IValidator<ProductAddRequest >  ProductAddRequestValidator) =>
               {
                 var validateResult=  ProductAddRequestValidator.Validate(productAddRequest);
                   if(!validateResult.IsValid)
                   {
                     Dictionary<string,string[]>errors=  validateResult.Errors.GroupBy(e => e.PropertyName).ToDictionary(
                           g => g.Key,
                           g => g.Select(e => e.ErrorMessage).ToArray()
                           );

                       return Results.ValidationProblem(errors);
                   }
                   ProductResponse? products = await productService.AddProduct(productAddRequest);

                   if(products!=null)
                   {
                     
                       // return Results.CreatedAtRoute($"api/prodcuts/productId/{products.ProductId}");
                       return Results.CreatedAtRoute(
            routeName: "api/prodcuts/productId/",
            routeValues: new { productId = products.ProductId },
            value: products.ProductId );
                   }
                   else
                   {
                       return Results.Problem("error in adding product");
                   }
                   
               });


            app.MapPut("/api/products",
              async (IProductsService productService, ProductUpdateRequest productupdateRequest, IValidator<ProductUpdateRequest> ProductAddRequestValidator) =>
              {
                  var validateResult = ProductAddRequestValidator.Validate(productupdateRequest);
                  if (!validateResult.IsValid)
                  {
                      Dictionary<string, string[]> errors = validateResult.Errors.GroupBy(e => e.PropertyName).ToDictionary(
                             g => g.Key,
                             g => g.Select(e => e.ErrorMessage).ToArray()
                             );

                      return Results.ValidationProblem(errors);
                  }
                  ProductResponse? products = await productService.UpdateProduct(productupdateRequest);

                  if (products != null)
                  {
                      return Results.Ok(products);
                  }
                  else
                  {
                      return Results.Problem("error in adding product");
                  }
              });

            app.MapDelete("/api/products/{productId:guid}",
             async (IProductsService productService, Guid productId, IValidator<ProductUpdateRequest> ProductAddRequestValidator) =>
             {
                
                
               var isDeleted = await productService.DeleteProduct(productId);

               return isDeleted ? Results.Ok(isDeleted) : Results.Problem("error in deleting product");
             });



            return app;
        }
    }
}
