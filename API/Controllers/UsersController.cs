using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
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
        private readonly IPhotoService _photoService;
      
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository,IMapper mapper
        ,IPhotoService photoService)
        {
          _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
           
        }

        //we add two end points to get specific users and all users from database
        [HttpGet]
       
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers
        ([FromQuery]UserParams userParams){



             var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            userParams.CurrentUsername = user.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender == "male" ? "female" : "male";

            var users = await _userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, 
                users.TotalCount, users.TotalPages);

            return Ok(users);
            //returns all registered users
        }

        //api/users/1 1 is id
       
        [HttpGet("{username}",Name ="GetUser")]

        public async Task<ActionResult<MemberDto>> GetUser(string username){
           var user= await _userRepository.GetMemberAsync(username);

           return user;

            //returns id matched user
        }

        [HttpPut] //To update on server we use put

        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto){
                var username=User.GetUsername();
                var user=await _userRepository.GetUserByUsernameAsync(username);
                _mapper.Map(memberUpdateDto,user);
                _userRepository.Update(user);
                if(await _userRepository.SaveAllAsync()) return NoContent();
                return BadRequest("Failed to Update Profile");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file){
               var user=await _userRepository.GetUserByUsernameAsync(User.GetUsername());
               var result=await _photoService.AddPhotoAsync(file);

               if(result.Error!=null) return BadRequest(result.Error.Message);

               var photo=new Photo{
                   Url=result.SecureUrl.AbsoluteUri,
                   PublicId=result.PublicId
               };

               if(user.Photos.Count==0){
                   photo.IsMain=true; //checks whether this is first photo or not
                   //If it is first photo set to  main
               }
               user.Photos.Add(photo);

               if(await _userRepository.SaveAllAsync()){
                //    return _mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("GetUser",new {username=user.UserName},_mapper.Map<PhotoDto>(photo));
                
               }
               return BadRequest("Problem Adding Photos");
        }


        [HttpPut("set-main-photo/{photoId}")]


        public async Task<ActionResult> SetMainPhoto(int photoId){
            var user=await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if(photo.IsMain) return BadRequest(("This is Already your Main Photo"));

            var currentMain=user.Photos.FirstOrDefault(x=>x.IsMain);
            if(currentMain!=null) currentMain.IsMain=false;
            photo.IsMain=true;

            if(await _userRepository.SaveAllAsync()) return NoContent();
        
            return BadRequest("Failed to set as Main Photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId){
            var user=await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);

            if(photo==null) return NotFound();

            if(photo.IsMain) return BadRequest("You cannot delete your Main Photo");

            if(photo.PublicId!=null){
                var result=await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error!=null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);


            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete Photo");
        }

    }
}