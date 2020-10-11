using AutoMapper;
using Contracts;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            this._logger = logger;
            this._mapper = mapper;
        }

        public IActionResult GetCompanies()
        {
            var companies = _repository.Company.GetAllCompanies(trackChanges: false);

            return Ok(_mapper.Map<IEnumerable<CompanyDto>>(companies));
        }
    }
}