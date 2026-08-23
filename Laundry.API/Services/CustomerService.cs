using Laundry.API.DTOs.Customer;
using Laundry.API.Entities;
using Laundry.API.Exceptions;
using Laundry.API.Interfaces;
using Laundry.API.Services.Interfaces;

namespace Laundry.API.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerResponse> CreateAsync(
    CreateCustomerRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var mobileNumber = request.MobileNumber.Trim();
        var email = request.Email.Trim();

        if (await _customerRepository.ExistsByMobileNumberAsync(mobileNumber))
            throw new DuplicateCustomerException(
                "A customer with this mobile number already exists.");

        if (await _customerRepository.ExistsByEmailAsync(email))
            throw new DuplicateCustomerException(
                "A customer with this email already exists.");

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            MobileNumber = mobileNumber,
            Email = email
        };

        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();

        return Map(customer);
    }

    public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
    {
        // GetAll will be added to the repository in the next step.
        var customers =
            await _customerRepository.GetAllAsync();

        return customers.Select(Map).ToList();
    }

    public async Task<CustomerResponse?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException(
                "Customer Id is invalid.",
                nameof(id));

        var customer =
            await _customerRepository.GetByIdAsync(id);

        return customer == null ? null : Map(customer);
    }

    public async Task<bool> UpdateAsync(
    Guid id,
    UpdateCustomerRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (id == Guid.Empty)
            throw new ArgumentException(
                "Customer Id is invalid.",
                nameof(id));

        var customer =
            await _customerRepository.GetByIdAsync(id);

        if (customer == null)
            return false;

        var mobileNumber = request.MobileNumber.Trim();
        var email = request.Email.Trim();

        if (await _customerRepository.ExistsByMobileNumberAsync(
                mobileNumber,
                id))
        {
            throw new DuplicateCustomerException(
                "A customer with this mobile number already exists.");
        }

        if (await _customerRepository.ExistsByEmailAsync(
                email,
                id))
        {
            throw new DuplicateCustomerException(
                "A customer with this email already exists.");
        }

        customer.FirstName = request.FirstName.Trim();
        customer.LastName = request.LastName.Trim();
        customer.MobileNumber = mobileNumber;
        customer.Email = email;
        customer.UpdatedOn = DateTime.Now;

        await _customerRepository.UpdateAsync(customer);
        await _customerRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException(
                "Customer Id is invalid.",
                nameof(id));

        var customer =
            await _customerRepository.GetByIdAsync(id);

        if (customer == null)
            return false;

        await _customerRepository.DeleteAsync(customer);
        await _customerRepository.SaveChangesAsync();

        return true;
    }

    private static CustomerResponse Map(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            MobileNumber = customer.MobileNumber,
            Email = customer.Email
        };
    }
}