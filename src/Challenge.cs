// DESAFIO: Chat em Grupo - SOLUÇÃO: Padrão Mediator

using System;
using System.Collections.Generic;

namespace DesignPatternChallenge
{
    public interface IChatMediator
    {
        void SendMessage(string sender, string message);
        void SendPrivateMessage(string sender, string recipient, string message);
        void Notify(string notification);
        void RegisterUser(ChatUser user);
        void MuteUser(string moderator, string target);
    }

    public class ChatMediator : IChatMediator
    {
        private readonly List<ChatUser> _users = new();

        public void RegisterUser(ChatUser user)
        {
            _users.Add(user);
            Notify($"{user.Name} entrou no grupo");
        }

        public void SendMessage(string sender, string message)
        {
            foreach (var u in _users)
                if (u.Name != sender && !u.IsMuted)
                    u.ReceiveMessage(sender, message);
        }

        public void SendPrivateMessage(string sender, string recipient, string message)
        {
            foreach (var u in _users)
                if (u.Name == recipient)
                    u.ReceivePrivateMessage(sender, message);
        }

        public void Notify(string notification)
        {
            foreach (var u in _users)
                u.ReceiveNotification(notification);
        }

        public void MuteUser(string moderator, string target)
        {
            foreach (var u in _users)
                if (u.Name == target) u.IsMuted = true;
            Notify($"{target} foi mutado por {moderator}");
        }
    }

    public class ChatUser
    {
        public string Name { get; set; }
        public bool IsMuted { get; set; }
        private readonly IChatMediator _mediator;

        public ChatUser(string name, IChatMediator mediator)
        {
            Name = name;
            _mediator = mediator;
        }

        public void SendMessage(string msg)
        {
            if (IsMuted) { Console.WriteLine($"[{Name}] ❌ Mutado"); return; }
            Console.WriteLine($"[{Name}] {msg}");
            _mediator.SendMessage(Name, msg);
        }

        public void SendPrivateMessage(string recipient, string msg)
        {
            if (IsMuted) { Console.WriteLine($"[{Name}] ❌ Mutado"); return; }
            Console.WriteLine($"[{Name}] → {recipient}: {msg}");
            _mediator.SendPrivateMessage(Name, recipient, msg);
        }

        public void ReceiveMessage(string from, string msg) => Console.WriteLine($"  → [{Name}] de {from}: {msg}");
        public void ReceivePrivateMessage(string from, string msg) => Console.WriteLine($"  → [{Name}] 🔒 de {from}: {msg}");
        public void ReceiveNotification(string n) => Console.WriteLine($"  → [{Name}] ℹ️ {n}");
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Chat (Mediator Pattern) ===\n");

            var mediator = new ChatMediator();
            var alice = new ChatUser("Alice", mediator);
            var bob = new ChatUser("Bob", mediator);
            var carlos = new ChatUser("Carlos", mediator);

            mediator.RegisterUser(alice);
            mediator.RegisterUser(bob);
            mediator.RegisterUser(carlos);

            alice.SendMessage("Olá!");
            bob.SendMessage("Oi!");
            alice.SendPrivateMessage("Bob", "Você viu o relatório?");
            mediator.MuteUser("Alice", "Carlos");
            carlos.SendMessage("Ainda posso?");
        }
    }
}
