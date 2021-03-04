using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiTask.Models.Dto;
using WebApiTask.Models.Entities;

namespace WebApiTask.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _usercon;
        public UserController(UserContext usercon)
        {
            _usercon = usercon;
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _usercon.Users.ToArray();
        }

        [HttpGet("{id}")]
        public ActionResult<User> Get(Int64 id)
        {
            var user = _usercon.Users.FirstOrDefault(p => p.Id == id);
            if (user is null) return NotFound();

            return user;
        }

        [HttpPost]
        public ActionResult Post(UserCreateDto userDto)
        {

            string tcNo = userDto.Id.ToString();
            int toplam = 0; int toplam2 = 0; int toplam3 = 0;
            if (tcNo.Length == 11)
            {
                if (Convert.ToInt32(tcNo[0].ToString()) != 0) //tc kimlik numaranın ilk hanesi 0 değilse
                {
                    for (int i = 0; i < 10; i++)
                    {
                        toplam = toplam + Convert.ToInt32(tcNo[i].ToString());
                        if (i % 2 == 0)
                        {
                            if (i != 10)
                            {
                                toplam2 = toplam2 + Convert.ToInt32(tcNo[i].ToString()); // 7 ile çarpılacak sayıları topluyoruz
                            }

                        }
                        else
                        {
                            if (i != 9)
                            {
                                toplam3 = toplam3 + Convert.ToInt32(tcNo[i].ToString());
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Tc Kimlik Numaranızın ilk hanesi 0 olamaz.");
                    return NotFound();
                }
            }
            else
            {
                Console.WriteLine("Tc Kimlik Numarınız 11 haneli olmak zorunda.Eksik ya da fazla değer girdiniz.");
                return NotFound();
            }
            if (((toplam2 * 7) - toplam3) % 10 == Convert.ToInt32(tcNo[9].ToString()) && toplam % 10 == Convert.ToInt32(tcNo[10].ToString()))
            {
                var user = new User { FirstName = userDto.FirstName, LastName = userDto.LastName, Birthday = userDto.Birthday, Address = userDto.Address, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Id = userDto.Id };
                _usercon.Users.Add(user);
                _usercon.SaveChanges();
                return CreatedAtAction("Get", new { id = user.Id }, user);
            }
            else
            {
                return NotFound();
            }
            
         


        }

        [HttpPut("{id}")]
        public ActionResult Put(Int64 id, UserUpdateDto userDto)
        {
            var user = _usercon.Users.FirstOrDefault(p => p.Id == id);
            if (user is null)
            {

                user = new User { FirstName = userDto.FirstName, LastName = userDto.LastName, Birthday = userDto.Birthday, Address = userDto.Address, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Id = id };
                _usercon.Users.Add(user);
                _usercon.SaveChanges();
                return CreatedAtAction("Get", new { id = id }, user);
            }
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Birthday = userDto.Birthday;
            user.Address = userDto.Address;
            user.ModifiedDate = DateTime.Now;
            _usercon.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Int64 id)
        {
            var author = _usercon.Users.FirstOrDefault(p => p.Id == id);
            if (author is null) return NotFound();

            _usercon.Remove(author);
            _usercon.SaveChanges();
            return NoContent();
        }
    }
}