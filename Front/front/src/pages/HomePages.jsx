function HomePages() {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
      <h1 className="text-4xl font-bold text-primary mb-4">Bienvenido a Comunicación</h1>
      <p className="text-lg text-secondary mb-8">Conéctate y comparte con tus amigos</p>
      <div className="flex space-x-4">
        <a href="/Registrar" className="px-6 py-2 bg-blue text-white rounded hover:bg-lightblue transition duration-300">
          Registrarse
        </a>
        <a href="/Ingresar" className="px-6 py-2 bg-primary text-white rounded hover:bg-lightblue transition duration-300">
          Ingresar
        </a>
      </div>
    </div>
  );
}

export default HomePages;
