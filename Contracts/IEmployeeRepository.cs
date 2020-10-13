﻿using Entities.Models;
using System;
using System.Collections.Generic;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges);

        Employee GetEmployee(Guid companyId, Guid employeeId, bool trackChanges);

        void CreateEmployeeForCompany(Guid companyId, Employee employee);
    }
}