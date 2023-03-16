using AutoMapper;
using Least.ExampleAPI.Domain;
using Least.ExampleAPI.Domain.Models;
using Least.ExampleAPI.Resources;

namespace Least.ExampleAPI.Mapping;

public class ModelToDb : Profile
{
    public ModelToDb()
    {
        CreateMap<User, UserModel>();
        CreateMap<Book, BookModel>();

        CreateMap<UserChangeRequest, User>()
            .ForMember(t => t.Id, o => o.Ignore());
        
        CreateMap<BookChangeRequest, Book>()
            .ForMember(t => t.Id, o => o.Ignore());
    }
}