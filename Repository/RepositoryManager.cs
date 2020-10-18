using Contracts;
using Entities;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private RepositoryContext _repositoryContext;
        private ICompanyRepository _companyRepo;
        private IEmployeeRepository _employeeRepo;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public ICompanyRepository Company => _companyRepo ?? new CompanyRepository(_repositoryContext);

        public IEmployeeRepository Employee => _employeeRepo ?? new EmployeeRepository(_repositoryContext);

        public Task SaveAsync() => _repositoryContext.SaveChangesAsync();
    }
}