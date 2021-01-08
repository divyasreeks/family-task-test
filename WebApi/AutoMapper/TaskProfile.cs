using Domain.Commands;
using Domain.DataModels;
using Domain.ViewModel;
using AutoMapper;

namespace WebApi.AutoMapper
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<CreateTaskCommand, Tasks>();
            CreateMap<UpdateTaskCommand, Tasks>();
            CreateMap<Tasks, TaskVm>();
        }
    }
}
