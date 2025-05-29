using DAL.Entities;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repository
{
    public class ProductReport:IDocument
    {
        public IEnumerable<Product> Products { get; set; }
        public ProductReport(IEnumerable<Product> Products)
        {
            this.Products = Products;
        }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Header().Text("Product Report").FontSize(20).Bold().AlignCenter();
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Name").Bold();
                        header.Cell().Text("Price").Bold();
                        header.Cell().Text("Description").Bold();
                    });

                    foreach (var product in Products)
                    {
                        table.Cell().Text(product.Name);    
                        table.Cell().Text(product.Price.ToString("C"));
                        table.Cell().Text(product.Description?.ToString());
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }
    }
}
