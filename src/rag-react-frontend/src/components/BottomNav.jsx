// src/components/BottomNav.jsx
import React from 'react';
import { NavLink } from 'react-router-dom';
import { motion } from 'framer-motion';

const NAV_ITEMS = [
  { name: 'Home',       to: '/'        },
  { name: 'About',      to: '/about'   },
  { name: 'Upload PDF', to: '/upload'   },
  { name: 'Ask me',     to: '/chat'    },
];

const dropSpring = { type: 'spring', stiffness: 250, damping: 18 };

export default function BottomNav() {
  return (
    <motion.nav
      initial={{ y: '-100vh', scale: 0.2, opacity: 0 }}
      animate={{ y: 0,       scale: 1,   opacity: 1 }}
      transition={{ duration: 1, delay: 2 , dropSpring}}
      
      className="fixed bottom-4 left-1/2 transform -translate-x-1/2 z-40 overflow-hidden"
    >
      <div className="bg-black px-3 py-3 rounded-full flex items-center">
        {NAV_ITEMS.map(item => (
          <NavLink
            key={item.to}
            to={item.to}
            end
            {...(item.to === '/' ? { reloadDocument: true } : {})}
            className={({ isActive }) =>
              `relative px-4 py-2 text-sm font-medium transition-colors cursor-none ${
                isActive ? 'text-black' : 'text-white'
              }`
            }
          >
            {({ isActive }) => (
              <>
                {isActive && (
                  <motion.div
                    layoutId="nav-pill"
                    className="absolute inset-0 bg-white rounded-full"
                    transition={{ type: 'spring', stiffness: 300, damping: 30 }}
                  />
                )}
                <span className="relative">{item.name}</span>
              </>
            )}
          </NavLink>
        ))}
      </div>
    </motion.nav>
  );
}
