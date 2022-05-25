using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context=context;
        }

        //we add two end points to get specific users and all users from database
        [HttpGet]

        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
            return await _context.Users.ToListAsync();
            //returns all registered users
        }

        //api/users/1 1 is id
        [HttpGet("{id}")]

        public async Task<ActionResult<AppUser>> GetUser(int id){
           return await _context.Users.FindAsync(id);

            //returns id matched user
        }

    }
}