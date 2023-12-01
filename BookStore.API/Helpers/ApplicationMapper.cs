using AutoMapper;
using BookStore.API.Data;
using BookStore.API.Model;

namespace BookStore.API.Helpers
{
    public class ApplicationMapper : Profile //mapper //profile is automapper class
    {
        public ApplicationMapper() {
                      //src  //dest
            CreateMap<Books ,BookModel>().ReverseMap();
            //CreateMap<BookModel, Books>();
        }

    }
}
