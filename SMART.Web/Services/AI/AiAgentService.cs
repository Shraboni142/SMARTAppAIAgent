using System;
using System.Collections.Generic;
using System.Linq;
using SMART.Web.Models.AI;
using SMART.Web.Repositories;
using SMART.Web.Repositories.AI;

namespace SMART.Web.Services.AI
{
    public interface IAiAgentService
    {
        AiChatSession CreateSession(string userId, string mode, string title);
        AiChatSession GetSessionById(int sessionId);
        List<AiChatSession> GetUserSessions(string userId);
        List<AiChatMessage> GetMessagesBySession(int sessionId);
        AiChatMessage SaveMessage(int sessionId, string userId, string mode, string userMessage, string aiReply);
        AiAgentSetting GetActiveSetting();
        void UpdateAiSetting(AiAgentSetting model);
        void SaveAiAgent();
    }

    public class AiAgentService : IAiAgentService
    {
        private readonly IAiChatSessionRepository _sessionRepository;
        private readonly IAiChatMessageRepository _messageRepository;
        private readonly IAiAgentSettingRepository _settingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AiAgentService(
            IAiChatSessionRepository sessionRepository,
            IAiChatMessageRepository messageRepository,
            IAiAgentSettingRepository settingRepository,
            IUnitOfWork unitOfWork)
        {
            _sessionRepository = sessionRepository;
            _messageRepository = messageRepository;
            _settingRepository = settingRepository;
            _unitOfWork = unitOfWork;
        }

        public AiChatSession CreateSession(string userId, string mode, string title)
        {
            var session = new AiChatSession
            {
                UserId = userId,
                Mode = mode,
                Title = title,
                CreatedOn = DateTime.Now,
                IsDeleted = false
            };

            _sessionRepository.Add(session);
            SaveAiAgent();

            return session;
        }

        public AiChatSession GetSessionById(int sessionId)
        {
            return _sessionRepository.Get(u => u.Id == sessionId && u.IsDeleted != true);
        }

        public List<AiChatSession> GetUserSessions(string userId)
        {
            return _sessionRepository
                .GetMany(u => u.UserId == userId && u.IsDeleted != true)
                .OrderByDescending(u => u.CreatedOn)
                .ToList();
        }
        public List<AiChatMessage> GetMessagesBySession(int sessionId)
        {
            return _messageRepository
                .GetMany(x => x.SessionId == sessionId && x.IsDeleted != true)
                .OrderBy(x => x.Id)
                .ToList();
        }
        public AiChatMessage SaveMessage(int sessionId, string userId, string mode, string userMessage, string aiReply)
        {
            var message = new AiChatMessage
            {
                SessionId = sessionId,
                UserId = userId,
                Mode = mode,
                UserMessage = userMessage,
                AiReply = aiReply,
                CreatedOn = DateTime.Now,
                IsDeleted = false
            };

            _messageRepository.Add(message);
            SaveAiAgent();

            return message;
        }
        public void UpdateAiSetting(AiAgentSetting model)
        {
            var setting = _settingRepository
                .GetMany(x => x.Id == model.Id)
                .FirstOrDefault();

            if (setting == null)
            {
                setting = new AiAgentSetting
                {
                    ProviderName = model.ProviderName,
                    ApiBaseUrl = model.ApiBaseUrl,
                    ApiKey = model.ApiKey,
                    ModelName = model.ModelName,
                    IsActive = model.IsActive,
                    CreatedOn = DateTime.Now
                };

                _settingRepository.Add(setting);
            }
            else
            {
                setting.ProviderName = model.ProviderName;
                setting.ApiBaseUrl = model.ApiBaseUrl;
                setting.ApiKey = model.ApiKey;
                setting.ModelName = model.ModelName;
                setting.IsActive = model.IsActive;
                setting.UpdatedOn = DateTime.Now;

                _settingRepository.Update(setting);
            }

            SaveAiAgent();
        }
        public AiAgentSetting GetActiveSetting()
        {
            return _settingRepository
                .GetMany(u => u.IsActive == true)
                .OrderByDescending(u => u.Id)
                .FirstOrDefault();
        }

        public void SaveAiAgent()
        {
            _unitOfWork.Commit();
        }
    }
}