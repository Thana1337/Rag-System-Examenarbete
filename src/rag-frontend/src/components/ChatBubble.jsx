export default function ChatBubble({ text, isUser }) {
  const baseClasses = 'backdrop-blur-md rounded-2xl px-5 py-3 shadow-lg max-w-lg';
  const userClasses = 'bg-accent/40 text-white self-end';
  const botClasses = 'bg-secondary/30 text-white self-start';

  return (
    <div className={`flex ${isUser ? 'justify-end' : 'justify-start'}`}>
      <div className={`${baseClasses} ${isUser ? userClasses : botClasses}`}>        
        {text}
      </div>
    </div>
  );
}