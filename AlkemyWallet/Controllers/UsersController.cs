using AlkemyWallet.Core.Helper;
using AlkemyWallet.Core.Interfaces;
using AlkemyWallet.Core.Models;
using AlkemyWallet.Core.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static AlkemyWallet.Core.Helper.Constants;

namespace AlkemyWallet.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UsersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    ///     Retrieve all users
    /// </summary>
    /// <param name="page">Page number starting in 1</param>
    /// <returns>User list</returns>
    [HttpGet(Name = "")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<UserDTO>> GetUsers(int page)
    {
        var result = await _userService.GetUsersPaging(page <= 0 ? page = PageListed.PAGE : page, PageListed.PAGESIZE);

        if (result.totalPages < page)
            return NotFound(PAGE_NOT_FOUND_MESSAGE);

        var resultDTO = _mapper.Map<IEnumerable<UserDTO>>(result.recordList);
        var pagedTransactions = new PageListed(page, result.totalPages);

        pagedTransactions.AddHeader(Response, Url.ActionLink(null, "Users", null, "https"));
        return Ok(resultDTO);
    }

    /// <summary>
    ///     Retrieve the user by their ID
    /// </summary>
    /// <param name="id"> The ID of the desired User</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Standard")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetById(id);   

        if (user is null)
            return NotFound(USER_NOT_FOUND_MESSAGE);

        var userForShow = _mapper.Map<UserByIdDTO>(user);
        return Ok(userForShow);
    }

    /// <summary>
    ///     Create a new user
    /// </summary>
    /// <param name="userDTO">Fields to create the user</param>
    /// <returns>If executed correctly, it returns a 200 response code.</returns>
    [HttpPost]
    [Authorize(Roles = "Standard")]
    public async Task<ActionResult> InsertUser(UserForCreatoionDto userDTO)
    {
        var result = await _userService.AddUser(userDTO);

        if (!result.Success) 
            return NotFound(result.Message);
      
        return Ok(result.Message);
       
    }

    /// <summary>
    ///     updates the user with the received in the request
    /// </summary>
    /// <param name="id">User Id</param>
    /// <param name="userDTO">Fields to update the user</param>
    /// <returns>If executed correctly, it returns a 200 response code.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Standard")]
    public async Task<ActionResult> UpdateUser(int id, UserForUpdateDto userDTO)
    {
        var result = await _userService.UpdateUser(id, userDTO);

        if (!result.Success)
            return NotFound(result.Message);

        return Ok(result.Message);
    }

    /// <summary>
    ///     delete the user with the id received in the request
    /// </summary>
    /// <param name="id">User Id</param>
    /// <returns>If executed correctly, it returns a 200 response code.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUser(id);

        if (!result)
            return NotFound(USER_NOT_FOUND_MESSAGE);

        return Ok(USER_DELETED_MESSAGE);
        
    }

    /// <summary>
    ///     Exchange the user's points for a product from the catalog.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("product/{id}")]
    [Authorize(Roles = "Standard")]
    public async Task<ActionResult> ExchangePoints(int id)
    {
        var userIdFromToken = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("uid"))!.Value;
        var result = await _userService.Exchange(id, userIdFromToken);

        if (!result.Success)
            return NotFound(result.Message);

        return Ok(result.Message);
    }
}