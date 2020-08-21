using MediatR;
using CaseFile.Api.Form.Models;

namespace CaseFile.Api.Form.Queries
{
    public class AddFormQuery : IRequest<FormDTO>
    {
        public FormDTO Form { get; set; }
        public int UserId { get; set; }
    }
}
