using System;
using System.Collections.Generic;
using System.Linq;
using WebClient.Abstractions;
using WebClient.Shared.Models;
using Core.Extensions.ModelConversion;
using System.Threading.Tasks;
using Domain.Commands;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Domain.ViewModel;
using Domain.Queries;
using System.Text.Json;

namespace WebClient.Services
{
    public class TaskDataService : ITaskDataService
    {
        private readonly HttpClient httpClient;
        public TaskDataService(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory.CreateClient("FamilyTaskAPI");
            tasks = new List<TaskVm>();
            LoadTasks();
        }

        public TaskVm SelectedTask { get; private set; }

        public IEnumerable<TaskVm> tasks;
        public IEnumerable<TaskVm> Tasks => tasks;


        public event EventHandler TasksUpdated;
        public event EventHandler TaskSelected;

        private async void LoadTasks()
        {
            tasks = (await GetAllTasks()).Payload;
            TasksUpdated?.Invoke(this, null);
        }

        
        public void SelectTask(Guid id)
        {
            if (tasks.All(taskVm => taskVm.Id != id)) return;
            {
                SelectedTask = tasks.SingleOrDefault(taskVm => taskVm.Id == id);
                TasksUpdated?.Invoke(this, null);
            }
        }

        

        public void ToggleTask(TaskVm model)
        {
            foreach (var taskModel in Tasks)
            {
                if (taskModel.Id == model.Id)
                {
                    model.IsComplete = !model.IsComplete;
                    UpdateTask(model);
                    taskModel.IsComplete = !taskModel.IsComplete;
                }
            }

            TasksUpdated?.Invoke(this, null);
        }

        private async Task<CreateTaskCommandResult> Create(CreateTaskCommand command)
        {
            try
            {
                return await httpClient.PostJsonAsync<CreateTaskCommandResult>("tasks", command);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        private async Task<GetAllTasksQueryResult> GetAllTasks()
        {
            return await httpClient.GetJsonAsync<GetAllTasksQueryResult>("tasks");
        }

        private async Task<UpdateTaskCommandResult> Update(UpdateTaskCommand command)
        {
            return await httpClient.PutJsonAsync<UpdateTaskCommandResult>($"tasks/{command.Id}", command);
        }


       

        public async Task AddTask(TaskVm model)
        {

            var result = await Create(model.ToCreateTaskCommand());
            if (result != null)
            {
                var updatedList = (await GetAllTasks()).Payload;

                if (updatedList != null)
                {
                    tasks = updatedList;
                    TasksUpdated?.Invoke(this, null);
                    return;
                }
            }

        }


        private async Task<GetAllTasksQueryResult> GetAllMembers()
        {
            return await httpClient.GetJsonAsync<GetAllTasksQueryResult>("tasks");
        }


        public void AddTask(TaskModel model)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateTask(TaskVm model)
        {
            var result = await Update(model.ToUpdateTaskCommand());
            Console.WriteLine(JsonSerializer.Serialize(result));

            if (result != null)
            {
                var updatedList = (await GetAllMembers()).Payload;

                if (updatedList != null)
                {
                    tasks = updatedList;
                    TasksUpdated?.Invoke(this, null);
                    return;
                }
            }
        }

        public async void LoadAllTasks()
        {
            tasks = (await GetAllTasks()).Payload;
            TasksUpdated?.Invoke(this, null);
        }
    }
}