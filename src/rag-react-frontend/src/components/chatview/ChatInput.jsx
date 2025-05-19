import React from 'react';
import { motion } from 'framer-motion';

export default function ChatInput({
  input,
  onChange,
  onKeyDown,
  sendMessage,
  loading
}) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.4, ease: 'easeOut' }}
      className="flex items-center p-4"
    >
      <textarea
        className="
          flex-1 resize-none border border-none bg-black/30 rounded-3xl p-3 py-4
          placeholder-white/70 text-white
          focus:outline-none focus:ring-1 focus:ring-white/40 cursor-none
        "
        rows={1}
        placeholder="Ask something..."
        value={input}
        onChange={onChange}
        onKeyDown={onKeyDown}
        disabled={loading}
      />
      <button
        onClick={sendMessage}
        disabled={loading || !input.trim()}
        className={`
          ml-3 px-6 py-4 rounded-4xl font-medium cursor-none
          ${loading || !input.trim()
            ? 'bg-black/30 text-white/60'
            : 'bg-white text-black hover:bg-white/75'}
        `}
      >
        {loading ? '…' : 'Send'}
      </button>
    </motion.div>
  );
}
