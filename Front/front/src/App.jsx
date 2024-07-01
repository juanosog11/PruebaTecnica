import { BrowserRouter, Routes, Route } from 'react-router-dom';
import RegistrarPages from "./pages/RegistrarPages";
import LoginPages from './pages/LoginPages';
import { AuthProvider } from "./context/AuthContext";
import HomePages from './pages/HomePages';
import WallPages from './pages/WallPages';
import PerfilPages from './pages/PerfilPages';
import ProtectedRoute from './ProtectedRoute';
import EditarPerfilPages from './pages/EditarPerfilPages';
import Navbar from './components/Navbar';
import SeguidoresPages from './pages/SeguidoresPages';

import PerfilOthersPages from './pages/PerfilOthersPages';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Navbar />
        <Routes>
          <Route path='/' element={<HomePages />} />
          <Route path='/Registrar' element={<RegistrarPages />} />
          <Route path='/Ingresar' element={<LoginPages />} />

          <Route element={<ProtectedRoute />}>
            <Route path='/wall' element={<WallPages />} />
            <Route path='/perfilUsuario' element={<PerfilOthersPages />} />
            <Route path='/perfil' element={<PerfilPages />} />
            <Route path='/seguidores' element={<SeguidoresPages />} />
            <Route path='/editarPerfil' element={<EditarPerfilPages />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
