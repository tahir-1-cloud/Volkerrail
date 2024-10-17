using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace PRISM.EventHandlersBLL
{
    public class HeaderFooterEventHandler : IEventHandler
    {
        private readonly string headerHtml;

        public HeaderFooterEventHandler(string headerHtml)
        {
            this.headerHtml = headerHtml;
        }

        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
            PdfDocument pdfDoc = docEvent.GetDocument();

            var pageSize = new iText.Kernel.Geom.PageSize(1450, 800);
            pageSize = (iText.Kernel.Geom.PageSize)pageSize.ApplyMargins(0, 0, 0, 0, false);
            pdfDoc.SetDefaultPageSize(pageSize);

            // Add header
            Document document = new Document(pdfDoc, pageSize);
            document.SetMargins(36, 36, 72, 36); // Adjust margins as needed

            Paragraph header = new Paragraph(headerHtml)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12)
                .SetMarginTop(10);
            document.Add(header);

            document.Close();
        }
    }

}
