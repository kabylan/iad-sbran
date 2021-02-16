﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sbran.Domain.Entities;
using Sbran.Domain.Models;

namespace Sbran.Domain.Data.Repositories.Contracts
{
    public interface IEmployeeRepository
    {
        Employee Create(Guid userId);

        Task DeleteAsync(Guid id);

        Task<List<Employee>> GetAllAsync();
        
        Task<List<Employee>> SearchAsync(string searchText);
        
        Task<Employee> GetAsync(Guid id);

        Task<Employee> GetByUserIdAsync(Guid userId);

        Task UpdateScientificInfoAsync(Guid employeeId, ScientificInfoDto scientificInfoDto);
        Task UpdateJobAsync(Guid employeeId, JobDto jobDto);
    }
}