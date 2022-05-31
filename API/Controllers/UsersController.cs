using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
        [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
      
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository,IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
           
        }

        //we add two end points to get specific users and all users from database
        [HttpGet]
       
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers(){
            var users=await _userRepository.GetMembersAsync();
            return Ok(users);
            //returns all registered users
        }

        //api/users/1 1 is id
       
        [HttpGet("{username}")]

        public async Task<ActionResult<MemberDto>> GetUser(string username){
           var user= await _userRepository.GetMemberAsync(username);

           return user;

            //returns id matched user
        }

        [HttpPut] //To update on server we use put

        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto){
                var username=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user=await _userRepository.GetUserByUsernameAsync(username);
                _mapper.Map(memberUpdateDto,user);
                _userRepository.Update(user);
                if(await _userRepository.SaveAllAsync()) return NoContent();
                return BadRequest("Failed to Update Profile");
        }

    }
}