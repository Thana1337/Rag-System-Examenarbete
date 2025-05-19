import { useState } from 'react';

export default function InputBox({ onSend }) {
  const [value, setValue] = useState('');
  return (
    <div className="p-4 bg-secondary/20 backdrop-blur-lg">
      <form
        onSubmit={(e) => {
          e.preventDefault();
          onSend(value);
          setValue('');
        }}
        className="flex space-x-4"
      >
        <input
          className="flex-1 bg-transparent border border-secondary/40 rounded-full px-6 py-3 focus:outline-none placeholder-secondary/60 text-white"
          placeholder="Type your message..."
          value={value}
          onChange={(e) => setValue(e.target.value)}
        />
        <button
          type="submit"
          className="bg-accent/50 hover:bg-accent/70 rounded-full px-8 py-3 text-white font-semibold shadow-md"
        >
          Send
        </button>
      </form>
    </div>
  );
}