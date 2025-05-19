// src/components/BackButton.jsx
import React from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';

export default function BackButton() {
  const navigate = useNavigate();

  return (
    <div className='container mx-auto m-1'>
    <motion.button
      onClick={() => navigate(-1)}
      className="
        flex items-center space-x-2
        text-white bg-white/10 hover:bg-white/20
        px-4 py-2 rounded-md cursor-none
      "
      whileHover={{ scale: 1.05 }}
      whileTap={{ scale: 0.95 }}
    >
      <svg
        className="w-5 h-5"
        fill="none"
        stroke="currentColor"
        strokeWidth="2"
        viewBox="0 0 24 24"
      >
        <path strokeLinecap="round" strokeLinejoin="round" d="M15 19l-7-7 7-7" />
      </svg>
    </motion.button>

    </div>
    
  );
}
