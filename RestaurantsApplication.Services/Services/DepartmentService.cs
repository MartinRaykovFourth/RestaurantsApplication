﻿using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.DepartmentDTOs;
using RestaurantsApplication.Services.Contracts;
using System.Collections;

namespace RestaurantsApplication.Services.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly RestaurantsContext _context;

        public DepartmentService(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentWithIdDTO>> GetDepartmentsWithIdsAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentWithIdDTO
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .ToListAsync();
        }

        public async Task AddDepartmentAsync(DepartmentShortInfoDTO dto)
        {
            var department = new Department()
            {
                Name = dto.Name,
                LocationId = dto.LocationId
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DepartmentWithLocationDTO>> GetAllAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentWithLocationDTO()
                {
                    Id = d.Id,
                    Name = d.Name,
                    LocationName = d.Location.Name
                })
                .ToListAsync();
        }

        public async Task<DepartmentFullInfoDTO> GetByIdAsync(int departmentId)
        {
            return await _context.Departments
                .Where(d => d.Id == departmentId)
                .Select(d => new DepartmentFullInfoDTO()
                {
                    Id = d.Id,
                    Name = d.Name,
                    LocationId = d.LocationId
                })
                .SingleOrDefaultAsync();
        }

        public async Task EditAsync(DepartmentFullInfoDTO dto)
        {
            var department = await _context.Departments
                .Where(d => d.Id == dto.Id)
                .SingleAsync();

            department.Name = dto.Name;
            department.LocationId = dto.LocationId;

            await _context.SaveChangesAsync();
        }
    }
}