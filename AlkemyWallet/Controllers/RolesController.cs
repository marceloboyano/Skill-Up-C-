﻿using AlkemyWallet.Core.Interfaces;
using AlkemyWallet.Core.Models;
using AlkemyWallet.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlkemyWallet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        public RolesController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        // Get all roles
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<RolesDTO>> GetRoles()
        {
            var roles = await _roleService.GetAllRoles();
            var rolesForShow = _mapper.Map<IEnumerable<RolesDTO>>(roles);
            return Ok(rolesForShow);

        }
        //Get rol by id
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var roles = await _roleService.GetRoleById(id);
            return Ok(roles);
        }
        [HttpPost]
        public async Task<ActionResult> InsertRole([FromForm] RoleDTO roleDTO)
        {
            await _roleService.AddRole(roleDTO);
            return Ok("The Role has been created successfully");
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRole(int id, [FromForm] RoleForUpdateDTO roleDTO)
        {
            var result = await _roleService.UpdateRole(id, roleDTO);
            if (!result) return NotFound("Role not found");
            return Ok("Successfully Modified Role");
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Role>> DeleteRole(int id)
        {

            var deleteRole = await _roleService.GetRoleById(id);
            if (deleteRole == null)
            {
                return NotFound($"Role with Id = {id} not found");
            }
            await _roleService.DeleteRole(id);
            return Ok($"Role with Id ={id} deleted");
        }
    }
}