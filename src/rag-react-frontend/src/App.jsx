// src/App.jsx
import React from 'react';
import { BrowserRouter, Routes, Route, useLocation } from 'react-router-dom';
import { AnimatePresence, motion } from 'framer-motion';

import CustomCursor from './components/CustomCursor';
import Header       from './components/header/Header';
import Home         from './components/Home';
import ChatView     from './components/chatview/ChatView';
import BottomNav from './components/BottomNav';
import About from './components/about/About';
import UploadPage from './components/upload/UploadPage';


const pageVariants = {
  initial: { opacity: 0, y: 20 },
  animate: { opacity: 1, y: 0 },
  exit:    { opacity: 0, y: -20 }
};

const pageTransition = { duration: 0.1, ease: 'easeInOut' };

function AnimatedRoutes() {
  const location = useLocation();

  return (
    <AnimatePresence mode="wait">
      <Routes location={location} key={location.pathname}>
        <Route
          path="/"
          element={
            <motion.div
              variants={pageVariants}
              initial="initial"
              animate="animate"
              exit="exit"
              transition={pageTransition}
              className="h-screen"
            >
              <Home/>
            </motion.div>
          }
        />
        <Route
          path="/chat"
          element={
            <motion.div
              variants={pageVariants}
              initial="initial"
              animate="animate"
              exit="exit"
              transition={pageTransition}
              className=""
            >
              <ChatView />
            </motion.div>
          }
        />
        <Route
          path="/about"
          element={
            <motion.div
              variants={pageVariants}
              initial="initial"
              animate="animate"
              exit="exit"
              transition={pageTransition}
              className=""
            >
              <About />
            </motion.div>
          }
        />
        <Route
          path="/upload"
          element={
            <motion.div
              variants={pageVariants}
              initial="initial"
              animate="animate"
              exit="exit"
              transition={pageTransition}
              className=""
            >
              <UploadPage />
            </motion.div>
          }
        />
      </Routes>
    </AnimatePresence>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <div className="cursor-none ">
        <CustomCursor />
        <Header />
        <BottomNav/>
        <AnimatedRoutes />
      </div>
    </BrowserRouter>
  );
}
