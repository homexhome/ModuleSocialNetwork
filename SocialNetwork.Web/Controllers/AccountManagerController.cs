using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Data;
using SocialNetwork.Models.Db;
using SocialNetwork.Models.ViewModels.Account;
using SocialNetwork.Models.ViewModels.Extensions;

namespace SocialNetwork.Web.Controllers
{
    public class AccountManagerController : Controller
    {
        private IMapper _mapper;
        private UserManager<User> _manager;
        private SignInManager<User> _signInManager;

        private IUnitOfWork _unitOfWork;

        public AccountManagerController(IMapper mapper, UserManager<User> manager, SignInManager<User> signInManager, IUnitOfWork unitOfWork) {
            _mapper = mapper;
            _manager = manager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login() {
            return View("Home/Login");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null) {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (ModelState.IsValid) {
                var user = _manager.FindByEmailAsync(model.Email).Result;

                if (user == null) {
                    return RedirectToAction("Index", "Home");
                }

                var result = await _signInManager.PasswordSignInAsync(userName: user.UserName,
                                                                      password: model.Password,
                                                                      isPersistent: model.RememberMe,
                                                                      lockoutOnFailure: false);
                if (result.Succeeded) {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) {
                        //return Redirect(model.ReturnUrl);
                        return RedirectToAction("Index", "Home");
                    }
                    else {
                        return RedirectToAction("MyPage", "AccountManager");
                    }
                }
                else {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [Route("MyPage")]
        [HttpGet]
        public async Task<IActionResult> MyPage() {
            var user = User;

            var result = await _manager.GetUserAsync(user);

            if (result == null) {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }

            var model = new UserViewModel(result);

            model.Friends = await GetAllFriend(model.User);

            return View("User", model);
        }

        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit() {
            var user = User;

            var result = await _manager.GetUserAsync(user);

            var editmodel = _mapper.Map<UserEditViewModel>(result);

            return View("Edit", editmodel);
        }

        [Authorize]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserEditViewModel model) {
            if (ModelState.IsValid) {
                var user = await _manager.FindByIdAsync(model.UserId);

                user.Convert(model);

                var result = await _manager.UpdateAsync(user);
                if (result.Succeeded) {
                    return RedirectToAction("MyPage", "AccountManager");
                }
                else {
                    return RedirectToAction("Edit", "AccountManager");
                }
            }
            else {
                ModelState.AddModelError("", "Некорректные данные");
                return View("Edit", model);
            }
        }

        [Route("UserList")]
        [HttpPost]
        public async Task<IActionResult> UserList(string search) {
            var model = await CreateSearch(search);
            return View("UserList", model);
        }

        private async Task<SearchViewModel> CreateSearch(string search) {
            var currentuser = User;
            string searchDelta = search;

            var result = await _manager.GetUserAsync(currentuser);

            if (search.IsNullOrEmpty()) {
                searchDelta = "_";
            }

            var list = _manager.Users.AsEnumerable().Where(x => x.GetFullName().ToLower().Contains(searchDelta.ToLower())).ToList();
            var withfriend = await GetAllFriend();

            var data = new List<UserWithFriendExt>();
            list.ForEach(x => {
                var t = _mapper.Map<UserWithFriendExt>(x);
                t.IsFriendWithCurrent = withfriend.Where(y => y.Id == x.Id || x.Id == result.Id).Count() != 0;
                data.Add(t);
            });

            var model = new SearchViewModel() {
                UserList = data
            };

            return model;
        }

        private async Task<List<User>> GetAllFriend(User user) {
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return await repository.GetFriendsByUserAsync(user);
        }

        private async Task<List<User>> GetAllFriend() {
            var user = User;

            var result = await _manager.GetUserAsync(user);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return await repository.GetFriendsByUserAsync(result);
        }

        [Route("AddFriend")]
        [HttpPost]
        public async Task<IActionResult> AddFriend(string id) {
            var currentuser = User;

            var result = await _manager.GetUserAsync(currentuser);

            var friend = await _manager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            await repository.AddFriendAsync(result, friend);

            return RedirectToAction("MyPage", "AccountManager");

        }

        [Route("DeleteFriend")]
        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string id) {
            var currentuser = User;

            var result = await _manager.GetUserAsync(currentuser);

            var friend = await _manager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            await repository.DeleteFriendAsync(result, friend);

            return RedirectToAction("MyPage", "AccountManager");

        }

        [Route("Chat")]
        [HttpPost]
        public async Task<IActionResult> Chat(string id) {
            var currentuser = User;

            var result = await _manager.GetUserAsync(currentuser);
            var friend = await _manager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = await repository.GetMessages(result, friend);

            var model = new ChatViewModel() {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };
            return View("Chat", model);
        }

        [Route("NewMessage")]
        [HttpPost]
        public async Task<IActionResult> NewMessage(string id, ChatViewModel chat) {

            var currentuser = User;

            var result = await _manager.GetUserAsync(currentuser);
            var friend = await _manager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var item = new Message() {
                Sender = result,
                Recipient = friend,
                Text = chat.NewMessage.Text,
            };
            await repository.Create(item);

            var mess = await repository.GetMessages(result, friend);

            var model = new ChatViewModel() {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };
            return View("Chat", model);

        }


        private async Task<ChatViewModel> GenerateChat(string id) {
            var currentuser = User;

            var result = await _manager.GetUserAsync(currentuser);
            var friend = await _manager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = await repository.GetMessages(result, friend);

            var model = new ChatViewModel() {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };

            return model;
        }

        [Route("Chat")]
        [HttpGet]
        public async Task<IActionResult> Chat() {

            var id = Request.Query["id"];

            var model = await GenerateChat(id);
            return View("Chat", model);
        }

        [Route("LoadMessages")]
        [HttpGet]
        public async Task<IActionResult> LoadMessages(string id) {
            // Получите текущего пользователя
            var currentUser = await _manager.GetUserAsync(User);

            // Получите друга по его идентификатору (id)
            var friend = await _manager.FindByIdAsync(id);

            // Получите сообщения между текущим пользователем и другом
            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;
            var messages = await repository.GetMessages(currentUser, friend);

            // Генерируйте HTML-код для сообщений, который будет возвращен на клиентскую сторону
            var messagesHtml = string.Join("<br>", messages.Select(message => $"{message.Sender.FirstName}: {message.Text}"));

            return Content(messagesHtml, "text/html");
        }

        [Route("Generate")]
        [HttpGet]
        public async Task<IActionResult> Generate() {

            var usergen = new GenetateUsers();
            var userlist = usergen.Populate(35);

            foreach (var user in userlist) {
                var result = await _manager.CreateAsync(user, "123456");

                if (!result.Succeeded)
                    continue;
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
