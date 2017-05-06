﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FootballAIGame.Web.GameServerService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="GameServerService.IGameServerService")]
    public interface IGameServerService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/AddToLookingForRandomOpponent", ReplyAction="http://tempuri.org/IGameServerService/AddToLookingForRandomOpponentResponse")]
        string AddToLookingForRandomOpponent(string userName, string ai);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/AddToLookingForRandomOpponent", ReplyAction="http://tempuri.org/IGameServerService/AddToLookingForRandomOpponentResponse")]
        System.Threading.Tasks.Task<string> AddToLookingForRandomOpponentAsync(string userName, string ai);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/RemoveFromLookingForRandomOpponent", ReplyAction="http://tempuri.org/IGameServerService/RemoveFromLookingForRandomOpponentResponse")]
        void RemoveFromLookingForRandomOpponent(string playerName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/RemoveFromLookingForRandomOpponent", ReplyAction="http://tempuri.org/IGameServerService/RemoveFromLookingForRandomOpponentResponse")]
        System.Threading.Tasks.Task RemoveFromLookingForRandomOpponentAsync(string playerName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/StartMatch", ReplyAction="http://tempuri.org/IGameServerService/StartMatchResponse")]
        string StartMatch(string userName1, string ai1, string userName2, string ai2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/StartMatch", ReplyAction="http://tempuri.org/IGameServerService/StartMatchResponse")]
        System.Threading.Tasks.Task<string> StartMatchAsync(string userName1, string ai1, string userName2, string ai2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/CancelMatch", ReplyAction="http://tempuri.org/IGameServerService/CancelMatchResponse")]
        void CancelMatch(string playerName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/CancelMatch", ReplyAction="http://tempuri.org/IGameServerService/CancelMatchResponse")]
        System.Threading.Tasks.Task CancelMatchAsync(string playerName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/GetPlayerCurrentMatchStep", ReplyAction="http://tempuri.org/IGameServerService/GetPlayerCurrentMatchStepResponse")]
        int GetPlayerCurrentMatchStep(string playerName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/GetPlayerCurrentMatchStep", ReplyAction="http://tempuri.org/IGameServerService/GetPlayerCurrentMatchStepResponse")]
        System.Threading.Tasks.Task<int> GetPlayerCurrentMatchStepAsync(string playerName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/PlanTournament", ReplyAction="http://tempuri.org/IGameServerService/PlanTournamentResponse")]
        void PlanTournament(int tournamentId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/PlanTournament", ReplyAction="http://tempuri.org/IGameServerService/PlanTournamentResponse")]
        System.Threading.Tasks.Task PlanTournamentAsync(int tournamentId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/RemoveFromRunningTournament", ReplyAction="http://tempuri.org/IGameServerService/RemoveFromRunningTournamentResponse")]
        void RemoveFromRunningTournament(string playerName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameServerService/RemoveFromRunningTournament", ReplyAction="http://tempuri.org/IGameServerService/RemoveFromRunningTournamentResponse")]
        System.Threading.Tasks.Task RemoveFromRunningTournamentAsync(string playerName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGameServerServiceChannel : FootballAIGame.Web.GameServerService.IGameServerService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GameServerServiceClient : System.ServiceModel.ClientBase<FootballAIGame.Web.GameServerService.IGameServerService>, FootballAIGame.Web.GameServerService.IGameServerService {
        
        public GameServerServiceClient() {
        }
        
        public GameServerServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GameServerServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GameServerServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GameServerServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string AddToLookingForRandomOpponent(string userName, string ai) {
            return base.Channel.AddToLookingForRandomOpponent(userName, ai);
        }
        
        public System.Threading.Tasks.Task<string> AddToLookingForRandomOpponentAsync(string userName, string ai) {
            return base.Channel.AddToLookingForRandomOpponentAsync(userName, ai);
        }
        
        public void RemoveFromLookingForRandomOpponent(string playerName) {
            base.Channel.RemoveFromLookingForRandomOpponent(playerName);
        }
        
        public System.Threading.Tasks.Task RemoveFromLookingForRandomOpponentAsync(string playerName) {
            return base.Channel.RemoveFromLookingForRandomOpponentAsync(playerName);
        }
        
        public string StartMatch(string userName1, string ai1, string userName2, string ai2) {
            return base.Channel.StartMatch(userName1, ai1, userName2, ai2);
        }
        
        public System.Threading.Tasks.Task<string> StartMatchAsync(string userName1, string ai1, string userName2, string ai2) {
            return base.Channel.StartMatchAsync(userName1, ai1, userName2, ai2);
        }
        
        public void CancelMatch(string playerName) {
            base.Channel.CancelMatch(playerName);
        }
        
        public System.Threading.Tasks.Task CancelMatchAsync(string playerName) {
            return base.Channel.CancelMatchAsync(playerName);
        }
        
        public int GetPlayerCurrentMatchStep(string playerName) {
            return base.Channel.GetPlayerCurrentMatchStep(playerName);
        }
        
        public System.Threading.Tasks.Task<int> GetPlayerCurrentMatchStepAsync(string playerName) {
            return base.Channel.GetPlayerCurrentMatchStepAsync(playerName);
        }
        
        public void PlanTournament(int tournamentId) {
            base.Channel.PlanTournament(tournamentId);
        }
        
        public System.Threading.Tasks.Task PlanTournamentAsync(int tournamentId) {
            return base.Channel.PlanTournamentAsync(tournamentId);
        }
        
        public void RemoveFromRunningTournament(string playerName) {
            base.Channel.RemoveFromRunningTournament(playerName);
        }
        
        public System.Threading.Tasks.Task RemoveFromRunningTournamentAsync(string playerName) {
            return base.Channel.RemoveFromRunningTournamentAsync(playerName);
        }
    }
}
