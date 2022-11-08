//Leal Cabrera Ricardo
using System;

//Requerimiento 1.- Actualizacion
//                  a) Agregar el residuo de la division en el porfactor
//                  b) Agregar en instruccion los incrementos de terminos de factor
//                      a++, a--, a+=1, a-=1, a*=1, a/=1, a%=1
//                      en donde el 1 puede ser una expresion.
//                  c) Programar el destructor de la clase lexico 
//                      para ejecutar el metodo cerrarArchivo()
//Requerimiento 2.- Actualizacion  
//                  a) Marcar errores semanticos cuando los incrementos de termino
//                     o incrementos de factor superen el rango de la variable
//                  b) Considerar el incisco b y c para el for
//                  c) Hacer funcionar el do() y while()
//Requerimiento 3.-
//                  a) Considerar las variables y los casteos de las expresiones matematicas en ensamblador
//                  b) Considerar el residuo de la division en ensamblador 
//                  c)Programar el printf y scanf
//Requerimiento 4.-                
//                  a)Programar el else en ensamblador
//                  b)Progrmar el for en ensamblador

//Requerimiento 5.- 
//                  a)Programar el while
//                  b)Programar el do while

namespace Semantica
{
    public class Lenguaje : Sintaxis, IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("Destructor");
        }
        int cIf;
        int cFor;
        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        public Lenguaje()
        {
            cIf = 0;
            cFor = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = 0;
            cFor = 0;
        }

        /*~Lenguaje()
        {
            Console.WriteLine("Destructor");
            cerrar();
        }
        */
        private void addVariable(string nombre, Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }
        private void displayVariables()
        {
            log.WriteLine();
            log.WriteLine("Variables declaradas: ");
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre() + " : " + v.getTipoDato() + "\n");    
            }
        }
        private void variablesASM()
        {
            asm.WriteLine(";Variables:");
            foreach (Variable v in variables)
            {
                asm.WriteLine("\t" + v.getNombre() + " DW ? ");
                log.WriteLine(v.getNombre() + " : " + v.getTipoDato() + "\n");   
            }
        }
        public void SetPosicion(long posicion)
        {
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(posicion, SeekOrigin.Begin);
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("#make_COM#");
            asm.WriteLine("include emu8086.inc");
            asm.WriteLine("ORG 100h");
            Libreria();
            Variables();
            variablesASM();
            Main();
            displayVariables();
            asm.WriteLine("RET");
            asm.WriteLine("DEFINE_PRINT_NUM");
            asm.WriteLine("DEFINE_PRINT_NUM_UNS");
            asm.WriteLine("DEFINE_SCAN_NUM");
            asm.WriteLine("END");
        }
        private void modVariable(string nombre, float nuevoValor)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre() == nombre)
                {
                    v.setValor(nuevoValor);
                }
            }
        }
        //Requerimiento 4.- Obtener el valor de la variable cuando se requiera y programar el metodo getValor()
        private float getValor(string nombreVariable)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre() == nombreVariable)
                {
                    return v.getValor();
                }
            }
            return 0;
        }
        private Variable.TipoDato getTipo(string nombreVariable)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre() == nombreVariable)
                {
                    return v.getTipoDato();
                }
            }
            return Variable.TipoDato.Char;
        }
        private bool existeVariable(string nombre){
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }
            return false;
        }

        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Libreria();
            }
        }

         //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char;
                switch (getContenido())
                {
                    case "int":
                        tipo = Variable.TipoDato.Int;
                        break;
                    case "float":  
                        tipo = Variable.TipoDato.Float;
                        break;        
                } 
                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }

         //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if(getClasificacion() == Tipos.Identificador) 
            {
                if(!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }else
                {
                    throw new Error("Error de sintaxis, variable duplicada <" +getContenido()+"> en linea: "+linea, log);
                }
            } 
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion = true)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
                
            }    
            match("}"); 
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "case" && getContenido() !=  "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion);
            }
            else if(getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if(getContenido() == "for")
            {
                For(evaluacion);
            }
            else if(getContenido() == "switch")
            {
                Switch(evaluacion);
            }
            else
            {
                Asignacion(evaluacion);
            }
        }
        private Variable.TipoDato evaluaNumero(float resultado)
        {
            if(resultado% 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            if(resultado <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if(resultado <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Char;
        }
        private bool evaluaSemantica(string variable , float resultado)
        {
            Variable.TipoDato tipoDato = getTipo(variable); 
            return false;
        }
        //convierteValor 
        private float convierteValor(float valor, Variable.TipoDato tipoDato)
        {
            if(tipoDato == Variable.TipoDato.Char)
            {
                return (char)valor % 256;
            }
            else if(tipoDato == Variable.TipoDato.Int)
            {
                return (int)valor % 65536;
            }
            return valor;
        }
        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion)
        {   
            string nombre = getContenido();
            if(!existeVariable(getContenido()))
            {
                throw new Error("Error: Variable inexistente '" + getContenido() + "' Encontrada en Linea: " + linea, log);
            }
            match(Tipos.Identificador); 
            dominante = Variable.TipoDato.Char;
            if (getClasificacion() == Tipos.IncrementoFactor || getClasificacion() == Tipos.IncrementoTermino )
            {
                //requerimiento 1 b)
                //requerimiento 1 c)
            }
            else
            
            log.WriteLine();
            log.Write(getContenido() + " = ");
            match(Tipos.Asignacion);

            Expresion();
            match(";");
            float resultado = stack.Pop();
            asm.WriteLine("POP AX");
            log.Write(" = " + resultado);
            log.WriteLine();
            if (dominante < evaluaNumero(resultado))
            {
                dominante = evaluaNumero(resultado);
            }


            if (dominante <= getTipo(nombre))
            {
                if (evaluacion)
                {
                    modVariable(nombre, resultado);
                    asm.WriteLine("MOV " + nombre + ", AX");
                }
            }
            else
            {
                throw new Error("Error de semantica: no podemos asignar un: <" + dominante + "> a un <" + getTipo(nombre) +  "> en linea  " + linea, log);
            }
            if(getTipo(nombre) == Variable.TipoDato.Char)
            {
                asm.WriteLine("MOV AH, 0");
            }
            modVariable(nombre, resultado);
            asm.WriteLine("MOV " + nombre + ", AX");
        } 

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            bool validaWhile;
            int posicionWhile = posicion;
            int lineaWhile = linea;
            string variable = getContenido();
            do
            {
                validaWhile = Condicion("");
                if (!evaluacion)
                {
                    validaWhile = evaluacion;
                }
                match(")");
                if (getContenido() == "{") 
                {
                    if(validaWhile)
                    {
                        BloqueInstrucciones(evaluacion);
                    }
                    else
                    {
                        BloqueInstrucciones(false);
                    }
                }
                else
                {
                    if(validaWhile)
                    {
                        Instruccion(evaluacion);
                    }
                    else
                    {
                        Instruccion(false);
                    }
                }
                if (validaWhile)
            {
                posicion = posicionWhile - variable.Length;
                linea = lineaWhile;
                setPosicion(posicion);
                NextToken();
            }
        }
        while (validaWhile);
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            bool validaDo = evaluacion;
            string variable;
            if (!evaluacion)
            {
                validaDo = false;
            }
            match("do");
            int posicionDo = posicion;
            int lineaDo = linea;
            do
            {
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion);
                }
                else
                {
                  Instruccion(evaluacion);
                } 
                match("while");
                match("(");
                variable = getContenido();
                validaDo = Condicion("");
                if (!evaluacion)
                {
                    validaDo = evaluacion;
                }
                else if (validaDo)
                {
                    posicion = posicionDo -1;
                    linea = lineaDo;
                    setPosicion(posicion);
                    NextToken();
                }
            }   while (validaDo);

            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            string etiquetaInicioFor = "InicioFor" + cFor;
            string etiquetaFinFor = "FinFor" + cFor++;
            asm.WriteLine(etiquetaInicioFor + ":");
            match("for");
            match("(");
            Asignacion(evaluacion);
            float valor = 0;
            string variable = getContenido();
            bool validaFor;
            int posicionFor = GuardarPosicion;
            int lineaFor = linea;
            do
            {
                validaFor = Condicion("");
                if (!evaluacion)
                {
                    validaFor = false;
                }
                match(";");
                match(Tipos.Identificador);
                valor = Incremento(variable, evaluacion);
                match(")");
                if(getContenido() == "{")
                {
                    BloqueInstrucciones(validaFor);
                }
                else
                {
                    Instruccion(validaFor);
                }
                if  (validaFor)
                {
                    GuardarPosicion = posicionFor - variable.Length;
                    linea = lineaFor;
                    SetPosicion(GuardarPosicion);
                    NextToken();
                    modVariable(getContenido(), valor);
                }
                asm.WriteLine(etiquetaFinFor);
            }  
                while(validaFor);
                asm.WriteLine(etiquetaFinFor + ":");
  
        }
        private void setPosicion(int posicionFor)
        {
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(posicionFor, SeekOrigin.Begin);
        }

        //Incremento -> Identificador ++ | --
       /* private void Incremento(bool evaluacion)
        {
            string Variable = getContenido();
            //Requerimiento 2 sino existe la variable levantar excepcion
            if(!existeVariable(getContenido()))
            {
                throw new Error("Error: Variable inexistente '" + getContenido() + "' Encontrada en Linea: " + linea, log);
            }
            match(Tipos.Identificador);
            float incremento = 0;
            float nuevoValor = getValor(getContenido());
            Variable.TipoDato tipoDato = getTipo(getContenido());
            dominante = tipoDato;
            if(getContenido() == "++")
            {
                match("++");
                if (evaluacion)
                {
                    modVariable(Variable,getValor(Variable)+ 1);
                }
            }
            else
            {
                match("--");
                if (evaluacion)
                {
                    modVariable(Variable,getValor(Variable)- 1);
                }
            }
        }
*/
private float Incremento(string variable, bool evaluacion)
        {
            //variable = getContenido();
            //Requerimiento 2.- Si no existe la variable levanta la excepcion
            if (existeVariable(variable) == false)
            {
                throw new Error("Error: Variable inexistente " + getContenido() + " en la linea: " + linea, log);
            }
            //match(Tipos.Identificador);

            //string incrementoValor;
            float incremento = 0;
            float nuevoValor = getValor(variable);
            Variable.TipoDato tipoDato = getTipo(variable);
            dominante = Variable.TipoDato.Char;


            if (getContenido() == "++")
            {
                match("++");
                if (evaluacion)
                {
                    nuevoValor = getValor(variable) + 1;
                }

            }
            else if (getContenido() == "--")
            {
                match("--");
                if (evaluacion)
                {
                    nuevoValor = getValor(variable) - 1;
                }

            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion();
                incremento = stack.Pop();

                if (evaluacion)
                {
                    nuevoValor = getValor(variable) + incremento;
                }

            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion();

                incremento = stack.Pop();

                if (evaluacion)
                {
                    nuevoValor = getValor(variable) - incremento;
                }

            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion();

                incremento = stack.Pop();

                if (evaluacion)
                {
                    nuevoValor = getValor(variable) * incremento;
                }

            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion();

                incremento = stack.Pop();

                if (evaluacion)
                {
                    nuevoValor = getValor(variable) / incremento;
                }

            }
            else if (getContenido() == "%=")
            {
                match("%=");
                Expresion();

                incremento = stack.Pop();

                if (evaluacion)
                {
                    nuevoValor = getValor(variable) % incremento;
                }

            }
            if (getTipo(variable) == Variable.TipoDato.Char && nuevoValor > 255)
            {
                throw new Error("Error: El valor de la variable " + variable + " excede el rango de un char", log);
            }
            else if (getTipo(variable) == Variable.TipoDato.Int && nuevoValor > 65535)
            {
                throw new Error("Error: El valor de la variable " + variable + " excede el rango de un int", log);
            }
            return nuevoValor;
        }
        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion)
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
            match(")");
            match("{");
            ListaDeCasos(evaluacion);
            if(getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion);  
                }
                else
                {
                    Instruccion(evaluacion);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion)
        {
            match("case");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
            match(":");
            ListaInstruccionesCase(evaluacion);
            if(getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if(getContenido() == "case")
            {
                ListaDeCasos(evaluacion);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta)
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            asm.WriteLine("POP BX");
            float e1 = stack.Pop();
            asm.WriteLine("POP AX");
            asm.WriteLine("CMP AX, BX");
            switch (operador)
            {
                case "==":
                    asm.WriteLine("JNE " + etiqueta);
                    return e1 == e2;
                case ">":
                    asm.WriteLine("JLE " + etiqueta);
                    return e1 > e2;
                case ">=":
                    asm.WriteLine("JL " + etiqueta);
                    return e1 >= e2;
                case "<":
                    asm.WriteLine("JGE " + etiqueta);
                    return e1 < e2;
                case "<=":
                    asm.WriteLine("JG " + etiqueta);
                    return e1 <= e2;
                default:
                    asm.WriteLine("JE " + etiqueta);
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            string etiquetaIf = "if " + ++cIf;
            string etiquetaElse = "else " + ++cIf;
            match("if");
            match("(");
            bool validarIf = Condicion(etiquetaIf);
            if (!evaluacion)
            {
                validarIf = evaluacion;
            }
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf);  
            }
            else
            {
                Instruccion(validarIf);
            }
            asm.WriteLine("JMP " + etiquetaElse);
            asm.WriteLine(etiquetaIf + ":");
            if (getContenido() == "else") //Requerimiento 4 
            {
                match("else");
                if (getContenido() == "{")
                {
                    if (evaluacion)
                    {
                        BloqueInstrucciones(!validarIf);  
                    }
                    else
                    {
                        BloqueInstrucciones(evaluacion);  
                    }
                }
                else
                {
                    if (evaluacion)
                    {
                        Instruccion(!validarIf); 
                    }
                    else
                    {
                        Instruccion(evaluacion);    
                    }
                }
            }
        asm.WriteLine(etiquetaElse + ":");
        }

        //Printf -> printf(cadena | expresion);
        private void Printf(bool evaluacion)
        {
            match("printf");
            match("("); 
            if (getClasificacion() == Tipos.Cadena) 
            {
                //Requerimiento 1.- Aqui se eliminan las comillas del resultado
                if(evaluacion)
                {   
                    setContenido(getContenido().TrimEnd('\"'));
                    setContenido(getContenido().TrimStart('\"'));
                    setContenido(getContenido().Replace("\\n", "\n"));
                    setContenido(getContenido().Replace("\\t", "\t"));
                    Console.Write(getContenido());
                }
                asm.WriteLine("PRINTN \"" + getContenido() + "\"");
                match(Tipos.Cadena);
            }
            else 
            {
                Expresion();
                float resultado = stack.Pop();
                asm.WriteLine("POP AX");
                Console.Write(resultado); //Requerimiento Codigo ASM para imprimir una variable
                asm.WriteLine("CALL PRINT_NUM");
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena, &identificador);
        private void Scanf(bool evaluacion)    
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            //Requerimiento 2.- Si no existe la variable levanta la excepcion
            if(!existeVariable(getContenido()))
            {
                throw new Error("Error: Variable inexistente '" + getContenido() + "' Encontrada en Linea: " + linea, log);
            }
            if(evaluacion)
            {
                string val = "" + Console.ReadLine();
                float n;
                if (float.TryParse(val, out n))
                {
                     modVariable(getContenido(), n);
                }
                else
                {
                     throw new Error("Error: A una variable Numerica no se le puede asignar un valor no numerico en la linea: " + linea, log);
                }
                asm.WriteLine("CALL SCAN_NUM");
                asm.WriteLine("MOV " + getContenido() + ", CX");
            }
            
            //Requerimiento 5.- Modificar el valor de la variable
            match(Tipos.Identificador);
            match(")");
            match(";");
        }

        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true);
        }

        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                switch(operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        asm.WriteLine("ADD AX, BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        asm.WriteLine("SUB AX, BX");
                        asm.WriteLine("PUSH AX");
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                //Requerimiento 1 a)
                switch(operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH DX");
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                if(dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                asm.WriteLine("MOV AX" + "," + getContenido());
                asm.WriteLine("PUSH AX");
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                stack.Push(getValor(getContenido()));
                if(!existeVariable(getContenido()))
                {
                    throw new Error("Error: Variable inexistente '" + getContenido() + "' Encontrada en Linea: " + linea, log);
                }
                log.WriteLine(getContenido() + " ");
                if (dominante < getTipo(getContenido()))
                {
                    dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));
                asm.WriteLine("PUSH AX");
                match(Tipos.Identificador);
            }
            else
            {
                bool  huboCasteo = false;
                Variable.TipoDato casteo = Variable.TipoDato.Char;
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    huboCasteo = true;
                    switch (getContenido())
                    {
                        case "char":
                            casteo = Variable.TipoDato.Char;
                            break;
                        case "int":
                            casteo = Variable.TipoDato.Int;
                            break;
                        case "float":
                            casteo = Variable.TipoDato.Float;
                            break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                if (huboCasteo)
                {
                    dominante = casteo;
                    float valor = stack.Pop();
                    asm.WriteLine("POP AX");
                    stack.Push(convierteValor(valor , casteo));
                    //Requerimiento 2
                    //Saco un elemento del stack
                    //Convierto ese valor al equivalente en casteo
                    //Ej: si el casteo es char y el pop regresa un 256...
                    //... el valor equivalente en casteo es un 0
                }

            }
        }
        
        float convierte(float valor, Variable.TipoDato casteo)
        {
            switch (casteo)
            {
                case Variable.TipoDato.Char:
                    return valor % 256;
                case Variable.TipoDato.Int:
                    return valor % 65536;
            }
            return valor;
        }
    }
}