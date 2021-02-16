using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Sbran.CQS.Read;
using Sbran.CQS.Read.Contracts;
using Sbran.CQS.Read.Results;
using Sbran.Domain.Models;
using Sbran.Shared.Contracts;
using System.Collections.Generic;
using Sbran.WebApp.Models;
using Sbran.Domain.Data.Adapters;
using Sbran.Domain.Data.Repositories.Contracts;


namespace Sbran.WebApp.Controllers
{
	///TODO: лучше назвать как ProfileEmployee
	/// <summary>
	/// Контроллер профиля
	/// </summary>
	[ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IReadCommand<ProfileResult> _profileReadCommand;
        private readonly EmployeeReadCommand _employeeReadCommand;

        private readonly ProfileWriteCommand _profileWriteCommand;

        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IEmployeeRepository _employeeRepository;


        public ProfileController(
            IReadCommand<ProfileResult> profileReadCommand,
            EmployeeReadCommand employeeReadCommand,
            ProfileWriteCommand profileWriteCommand,
            IProfileRepository profileRepository,
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository)
        {
            _profileReadCommand = profileReadCommand;
            _employeeReadCommand = employeeReadCommand;
            _profileWriteCommand = profileWriteCommand;


            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        [Route("{profileId:guid}/employee/{employeeId:guid}")]
        public async Task<IActionResult> GetById(Guid profileId, Guid employeeId)
        {
            var userInfo = await _GetById(profileId, employeeId);

            var objectJson = JsonSerializer.SerializeToUtf8Bytes(userInfo, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var mediaType = new MediaTypeHeaderValue("application/octet-stream");
            var result = new FileContentResult(objectJson, mediaType);

            return result;
        }

        [HttpPost]
        [Route("{profileId:guid}")]
        public Task UpdateAsync(Guid profileId, ProfileDto profileDto)
        {
            return _profileWriteCommand.UpdateAsync(profileId, profileDto);
        }



        // GET api/account/list
        [HttpGet]
        [Route("[action]/{searchText}")]
        public async Task<IActionResult> Search(string searchText)
        {
            var usersFound = await _userRepository.GetByUserName(searchText);
            var employeesFound = await _employeeRepository.SearchAsync(searchText);

            var userInfoList = new List<UserInfoResult>();

            foreach (var user in usersFound)
            {
                var profileId = await _userRepository.GetProfileId(user.Id);
                var employeeId = await _userRepository.GetEmployeeId(user.Id);

                var userInfo = await _GetById(profileId, employeeId);
                userInfoList.Add(userInfo);
            }

            foreach (var employee in employeesFound)
            {
                var profileId = await _userRepository.GetProfileId(employee.UserId);

                var userInfo = await _GetById(profileId, employee.Id);
                userInfoList.Add(userInfo);
            }


            var objectJson = JsonSerializer.SerializeToUtf8Bytes(userInfoList, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var mediaType = new MediaTypeHeaderValue("application/octet-stream");
            var result = new FileContentResult(objectJson, mediaType);

            return result;
        }


        private async Task<UserInfoResult> _GetById(Guid profileId, Guid employeeId)
        {
            var profileResult = await _profileReadCommand.ExecuteAsync(profileId);
            var employeeResult = await _employeeReadCommand.ExecuteAsync(employeeId);

            var userInfo = new UserInfoResult
            {
                Profile = profileResult,
                ShortName = employeeResult.Organization?.ShortName,
                AcademicDegree = employeeResult.AcademicDegree,
                AcademicRank = employeeResult.AcademicRank,
                Education = employeeResult.Education,
                Fio = employeeResult.Passport?.ToFio(),
                Email = employeeResult.Contact?.Email,
                /// TODO: реализовать получение и заполнение факсов + база
                Fax = null,
                MobilePhoneNumber = employeeResult.Contact?.MobilePhoneNumber,
                WorkPlace = employeeResult.WorkPlace,
                Position = employeeResult.Position
            };

            return userInfo;

        }
    }
}
